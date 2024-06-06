using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SQLite;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    //
    // Init Db
    //
    private SQLiteConnection db;
    public GuidesService GuidesService { get; private set; }
    public StepsService StepsService { get; private set; }
    public SubStepsService SubStepsService { get; private set; }
    public DungeonsService DungeonsService { get; private set; }
    public QuestsService QuestsService { get; private set; }
    public NpcsService NpcsService { get; private set; }
    public ItemsService ItemsService { get; private set; }
    public ItemSubStepsService ItemSubStepsService { get; private set; }
    public MonstersService MonstersService { get; private set; }
    public MonsterSubStepsService MonsterSubStepsService { get; private set; }
    public UsersService UsersService { get; private set; }

    void Awake()
    {
        string databasePath = Path.Combine(Application.persistentDataPath, "GanyBase.db");

        // Check if the database exists
        if (!File.Exists(databasePath))
        {
            // Destroy PlayerPref updatedAt if database does not exist
            PlayerPrefs.DeleteKey("dofusDB_updated_at");
            PlayerPrefs.DeleteKey("guides_updated_at");
        }

        db = new SQLiteConnection(databasePath);

        // Initialize services
        GuidesService = new GuidesService(db);
        StepsService = new StepsService(db);
        SubStepsService = new SubStepsService(db);
        DungeonsService = new DungeonsService(db);
        QuestsService = new QuestsService(db);
        ItemsService = new ItemsService(db);
        ItemSubStepsService = new ItemSubStepsService(db);
        NpcsService = new NpcsService(db);
        MonstersService = new MonstersService(db);
        MonsterSubStepsService = new MonsterSubStepsService(db);
        UsersService = new UsersService(db);

        // Create tables if they don't exist
        db.CreateTable<Guides>();
        db.CreateTable<Steps>();
        db.CreateTable<SubSteps>();
        db.CreateTable<Dungeons>();
        db.CreateTable<Quests>();
        db.CreateTable<Items>();
        db.CreateTable<ItemSubSteps>();
        db.CreateTable<Npcs>();
        db.CreateTable<Monsters>();
        db.CreateTable<MonsterSubSteps>();
        db.CreateTable<Users>();

        StartCoroutine(SyncAndThenSyncGuides());
    }

    void OnDestroy()
    {
        db.Close();
    }

    //
    // SYNCHRO API TO Db
    //
    private IEnumerator SyncAndThenSyncGuides()
    {
        string lastUpdatedAt = PlayerPrefs.GetString("dofusDB_updated_at", null);
        string url = "https://ganymede-dofus.com/api/sync";
        if (!string.IsNullOrEmpty(lastUpdatedAt))
        {
            url += "?updated_at=" + lastUpdatedAt;
        }

        yield return StartCoroutine(SyncDataFromApi(url));
        PlayerPrefs.SetString("dofusDB_updated_at", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
        PlayerPrefs.Save();

        StartSyncGuides();
        PlayerPrefs.SetString("guides_updated_at", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
        PlayerPrefs.Save();
    }

    private IEnumerator SyncDataFromApi(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                ProcessApiResponse(webRequest.downloadHandler.text);
            }
        }
    }

    private void ProcessApiResponse(string jsonResponse)
    {
        JObject jsonObj = JObject.Parse(jsonResponse);

        List<Quests> questsToInsert = new List<Quests>();
        List<Monsters> monstersToInsert = new List<Monsters>();
        List<Dungeons> dungeonsToInsert = new List<Dungeons>();
        List<Items> itemsToInsert = new List<Items>();
        List<Npcs> npcsToInsert = new List<Npcs>();

        if (jsonObj["quests"] is JArray questsArray)
        {
            foreach (JObject quest in questsArray)
            {
                Quests newQuest = PrepareQuest(quest);
                if (newQuest != null)
                {
                    questsToInsert.Add(newQuest);
                }
            }
        }

        if (jsonObj["npcs"] is JArray npcsArray)
        {
            foreach (JObject npc in npcsArray)
            {
                Npcs newNpc = PrepareNpc(npc);
                if (newNpc != null)
                {
                    npcsToInsert.Add(newNpc);
                }
            }
        }

        if (jsonObj["monsters"] is JArray monstersArray)
        {
            foreach (JObject monster in monstersArray)
            {
                Monsters newMonster = PrepareMonster(monster);
                if (newMonster != null)
                {
                    monstersToInsert.Add(newMonster);
                }
            }
        }

        if (jsonObj["dungeons"] is JArray dungeonsArray)
        {
            foreach (JObject dungeon in dungeonsArray)
            {
                Dungeons newDungeon = PrepareDungeon(dungeon);
                if (newDungeon != null)
                {
                    dungeonsToInsert.Add(newDungeon);
                }
            }
        }

        if (jsonObj["items"] is JArray itemsArray)
        {
            foreach (JObject item in itemsArray)
            {
                Items newItem = PrepareItem(item);
                if (newItem != null)
                {
                    itemsToInsert.Add(newItem);
                }
            }
        }

        db.RunInTransaction(() =>
        {
            db.InsertAll(questsToInsert);
            db.InsertAll(monstersToInsert);
            db.InsertAll(dungeonsToInsert);
            db.InsertAll(itemsToInsert);
            db.InsertAll(npcsToInsert);
        });
    }

    private Quests PrepareQuest(JObject quest)
    {
        int apiId = (int)quest["id"];
        Quests existingQuest = QuestsService.GetQuestByApiId(apiId);
        if (existingQuest != null)
        {
            Debug.Log($"Quest already exists: {existingQuest.name}");
            return null;
        }
        else
        {
            return new Quests
            {
                dofusDbId = (int)quest["dofusdb_id"],
                apiId = apiId,
                name = (string)quest["name"]
            };
        }
    }

    private Npcs PrepareNpc(JObject npc)
    {
        int apiId = (int)npc["id"];
        Npcs existingNpc = NpcsService.GetNpcByApiId(apiId);
        if (existingNpc != null)
        {
            Debug.Log($"Npc already exists: {existingNpc.name}");
            return null;
        }
        else
        {
            return new Npcs
            {
                dofusDbId = (int)npc["dofusdb_id"],
                apiId = apiId,
                name = (string)npc["name"],
                imageUrl = (string)npc["image_url"]
            };
        }
    }

    private Monsters PrepareMonster(JObject monster)
    {
        int apiId = (int)monster["id"];
        Monsters existingMonster = MonstersService.GetMonsterByApiId(apiId);
        if (existingMonster != null)
        {
            Debug.Log($"Monster already exists: {existingMonster.name}");
            return null;
        }
        else
        {
            return new Monsters
            {
                dofusDbId = (int)monster["dofusdb_id"],
                apiId = apiId,
                name = (string)monster["name"],
                imageUrl = (string)monster["image_url"]
            };
        }
    }

    private Dungeons PrepareDungeon(JObject dungeon)
    {
        int apiId = (int)dungeon["id"];
        Dungeons existingDungeon = DungeonsService.GetDungeonByApiId(apiId);
        if (existingDungeon != null)
        {
            Debug.Log($"Dungeon already exists: {existingDungeon.name}");
            return null;
        }
        else
        {
            return new Dungeons
            {
                dofusDbId = (int)dungeon["dofusdb_id"],
                apiId = apiId,
                name = (string)dungeon["name"],
                level = (int)dungeon["level"]
            };
        }
    }

    private Items PrepareItem(JObject item)
    {
        int apiId = (int)item["id"];
        Items existingItem = ItemsService.GetItemByApiId(apiId);
        if (existingItem != null)
        {
            Debug.Log($"Item already exists: {existingItem.name}");
            return null;
        }
        else
        {
            return new Items
            {
                dofusDbId = (int)item["dofusdb_id"],
                apiId = apiId,
                name = (string)item["name"],
                imageUrl = (string)item["image_url"]
            };
        }
    }

    public void StartSyncGuides()
    {
        string lastUpdatedAt = PlayerPrefs.GetString("guides_updated_at", null);

        string url = "https://ganymede-dofus.com/api/guides";
        if (!string.IsNullOrEmpty(lastUpdatedAt))
        {
            url += "?updated_at=" + lastUpdatedAt;
        }

        StartCoroutine(SyncGuides(url));
    }

    // sync guides api/guides just the Guide Model
    private IEnumerator SyncGuides(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Received Guides List: " + webRequest.downloadHandler.text);
                JArray guidesArray = JArray.Parse(webRequest.downloadHandler.text);
                List<int> receivedGuideIds = new List<int>();
                List<Guides> guidesToInsert = new List<Guides>();

                foreach (JObject guide in guidesArray)
                {
                    int guideId = (int)guide["id"];
                    string updatedAt = (string)guide["updated_at"];
                    Guides existingGuide = GuidesService.GetGuideByApiId(guideId);

                    if (existingGuide == null || existingGuide.updatedAt != updatedAt)
                    {
                        Guides newGuide = PrepareGuide(guide);
                        if (newGuide != null)
                        {
                            guidesToInsert.Add(newGuide);
                        }
                    }
                    receivedGuideIds.Add(guideId);
                }

                // Remove guides not present in the response
                var existingGuides = GuidesService.GetAllGuides();
                foreach (var existingGuide in existingGuides)
                {
                    if (!receivedGuideIds.Contains(existingGuide.apiId))
                    {
                        GuidesService.DeleteGuideWithRelations(existingGuide.id);
                    }
                }

                // Insert or update guides in the database
                db.RunInTransaction(() =>
                {
                    foreach (var guide in guidesToInsert)
                    {
                        if (guide.id == 0)
                        {
                            db.Insert(guide);
                        }
                        else
                        {
                            db.Update(guide);
                        }
                    }
                });
            }
        }
    }

    private Guides PrepareGuide(JObject guide)
    {
        int apiId = (int)guide["id"];
        Guides existingGuide = GuidesService.GetGuideByApiId(apiId);
        if (existingGuide != null && existingGuide.updatedAt == (string)guide["updated_at"])
        {
            Debug.Log($"Guide already up to date: {existingGuide.name}");
            return null;
        }
        else
        {
            return new Guides
            {
                apiId = apiId,
                userId = (int)guide["user_id"],
                name = (string)guide["name"],
                description = (string)guide["description"],
                status = (string)guide["status"],
                updatedAt = (string)guide["updated_at"]
            };
        }
    }

    //Get A Full guide from API and save it to DB 
    public IEnumerator getAPIGuide(int id)
    {
        string url = $"https://ganymede-dofus.com/api/guides/{id}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log($"Received Guide {id}: " + webRequest.downloadHandler.text);
                LoadDataFromJson(webRequest.downloadHandler.text);
            }
        }
    }

    //
    // CREATION GUIDE
    //
    public void LoadDataFromJson(string jsonResponse)
    {
        Debug.Log("Loading JSON Data");

        try
        {
            JObject jsonObj = JObject.Parse(jsonResponse);

            if (jsonObj == null)
            {
                Debug.LogError("Failed to parse JSON or JSON is empty");
                return;
            }

            int apiId = (int)jsonObj["id"];
            Guides existingGuide = GuidesService.GetGuideByApiId(apiId);
            bool guideUpdated = existingGuide != null && existingGuide.updatedAt != (string)jsonObj["updated_at"];

            if (existingGuide == null || guideUpdated)
            {
                if (existingGuide != null)
                {
                    // Delete data and relations under guide
                    GuidesService.DeleteUnderGuideWithRelations(existingGuide.id);
                }

                Guides guide = new Guides
                {
                    apiId = apiId,
                    name = (string)jsonObj["name"],
                    description = (string)jsonObj["description"],
                    status = (string)jsonObj["status"],
                    updatedAt = (string)jsonObj["updated_at"]
                };

                if (existingGuide == null)
                {
                    db.Insert(guide);  // Ensure the Guide object is inserted to obtain its ID
                }
                else
                {
                    guide.id = existingGuide.id; // Keep the existing ID
                    db.Update(guide);
                }

                if (jsonObj["steps"] is JArray stepsArray)
                {
                    guide.steps = new List<Steps>();
                    foreach (JObject step in stepsArray)
                    {
                        Steps newStep = new Steps
                        {
                            guideId = guide.id,  // Use the Guide ID after insertion
                            apiId = (int)step["id"],
                            map = (string)step["map"],
                            name = (string)step["name"],
                            position = (int)step["order"],
                            posX = (int)step["pos_x"],
                            posY = (int)step["pos_y"],
                            updatedAt = (string)step["updated_at"]
                        };

                        db.Insert(newStep);

                        newStep.subSteps = new List<SubSteps>();
                        if (step["sub_steps"] is JArray subStepsArray)
                        {
                            foreach (JObject subStep in subStepsArray)
                            {
                                SubSteps newSubStep = new SubSteps
                                {
                                    stepId = newStep.id,
                                    type = (string)subStep["type"],
                                    apiId = (int)subStep["id"],
                                    position = (int)subStep["order"],
                                    text = (string)subStep["text"],
                                    updatedAt = (string)subStep["updated_at"]
                                };

                                db.Insert(newSubStep);

                                // Handle links to specific objects like Items or Monsters
                                if ((string)subStep["type"] == "item")
                                {
                                    if (subStep["items"] is JArray itemsArray)
                                    {
                                        ProcessItems(itemsArray, newSubStep);
                                    }
                                }
                                else if ((string)subStep["type"] == "monster")
                                {
                                    if (subStep["monsters"] is JArray monstersArray)
                                    {
                                        ProcessMonsters(monstersArray, newSubStep);
                                    }
                                }
                                else if ((string)subStep["type"] == "dungeon" || (string)subStep["type"] == "quest")
                                {
                                    ProcessContent(subStep, newSubStep);
                                }

                                newStep.subSteps.Add(newSubStep);
                            }
                        }

                        guide.steps.Add(newStep);
                    }
                }

                Debug.Log("Guide loaded and saved successfully.");
            }
            else
            {
                Debug.Log("Guide is already up to date.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading JSON: " + ex.Message);
        }
    }

    private void ProcessItems(JArray itemsArray, SubSteps newSubStep)
    {
        if (itemsArray == null) return;

        newSubStep.itemSubSteps = new List<ItemSubSteps>();
        foreach (JObject item in itemsArray)
        {
            int itemId;
            Items existingItem = ItemsService.GetItemByApiId((int)item["id"]);
            if (existingItem != null)
            {
                itemId = existingItem.id;
            }
            else
            {
                Debug.LogError($"Item with API ID {item["id"]} not found.");
                continue;
            }

            ItemSubSteps newItemSubStep = new ItemSubSteps
            {
                subStepId = newSubStep.id,
                itemId = itemId,
                apiId = (int)item["pivot"]["sub_step_id"],
                quantity = (int)item["pivot"]["quantity"],
                updatedAt = newSubStep.updatedAt
            };

            db.Insert(newItemSubStep); // Insert itemSubSteps into the database
            newSubStep.itemSubSteps.Add(newItemSubStep);
        }
    }

    private void ProcessMonsters(JArray monstersArray, SubSteps newSubStep)
    {
        if (monstersArray == null) return;

        newSubStep.monsterSubSteps = new List<MonsterSubSteps>();
        foreach (JObject monster in monstersArray)
        {
            int monsterId;
            Monsters existingMonster = MonstersService.GetMonsterByApiId((int)monster["id"]);
            if (existingMonster != null)
            {
                monsterId = existingMonster.id;
            }
            else
            {
                Debug.LogError($"Monster with API ID {monster["id"]} not found.");
                continue;
            }

            MonsterSubSteps newMonsterSubStep = new MonsterSubSteps
            {
                subStepId = newSubStep.id,
                monsterId = monsterId,
                apiId = (int)monster["pivot"]["sub_step_id"],
                quantity = (int)monster["pivot"]["quantity"],
                updatedAt = newSubStep.updatedAt
            };

            db.Insert(newMonsterSubStep); // Insert MonsterSubSteps into the database
            newSubStep.monsterSubSteps.Add(newMonsterSubStep);
        }
    }

    private void ProcessContent(JObject subStep, SubSteps newSubStep)
    {
        if ((string)subStep["type"] == "dungeon")
        {
            if (subStep["content"] is JObject content)
            {
                int dungeonId;
                Dungeons existingDungeon = DungeonsService.GetDungeonByApiId((int)content["id"]);
                if (existingDungeon != null)
                {
                    dungeonId = existingDungeon.id;
                }
                else
                {
                    Debug.LogError($"Dungeon with API ID {content["id"]} not found.");
                    return;
                }

                newSubStep.dungeonContentId = dungeonId;
                newSubStep.dungeonContent = existingDungeon;
                db.Update(newSubStep);  // Update SubSteps after inserting Dungeon
            }
        }
        else if ((string)subStep["type"] == "quest")
        {
            if (subStep["content"] is JObject content)
            {
                int questId;
                Quests existingQuest = QuestsService.GetQuestByApiId((int)content["id"]);
                if (existingQuest != null)
                {
                    questId = existingQuest.id;
                }
                else
                {
                    Debug.LogError($"Quest with API ID {content["id"]} not found.");
                    return;
                }

                newSubStep.questContentId = questId;
                newSubStep.questContent = existingQuest;
                db.Update(newSubStep);  // Update SubSteps after inserting Quest
            }
        }
        else if ((string)subStep["type"] == "npc")
        {
            if (subStep["content"] is JObject content)
            {
                int npcId;
                Npcs existingNpc = NpcsService.GetNpcByApiId((int)content["id"]);
                if (existingNpc != null)
                {
                    npcId = existingNpc.id;
                }
                else
                {
                    Debug.LogError($"Npc with API ID {content["id"]} not found.");
                    return;
                }

                newSubStep.npcContentId = npcId;
                newSubStep.npcContent = existingNpc;
                db.Update(newSubStep);  // Update SubSteps after inserting Quest
            }
        }
    }
}
