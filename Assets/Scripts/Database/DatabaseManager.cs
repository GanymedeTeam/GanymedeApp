using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SQLite;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class DatabaseManager : MonoBehaviour
{
    private SQLiteConnection db;
    public GuidesService GuidesService { get; private set; }
    public StepsService StepsService { get; private set; }
    public SubStepsService SubStepsService { get; private set; }
    public DungeonsService DungeonsService { get; private set; }
    public QuestsService QuestsService { get; private set; }
    public ItemsService ItemsService { get; private set; }
    public ItemSubStepsService ItemSubStepsService { get; private set; }
    public MonstersService MonstersService { get; private set; }
    public MonsterSubStepsService MonsterSubStepsService { get; private set; }
    public UsersService UsersService { get; private set; }

    void Awake()
    {
        string databasePath = Path.Combine(Application.persistentDataPath, "GanyBase.db");
        db = new SQLiteConnection(databasePath);

        // Initialize services
        GuidesService = new GuidesService(db);
        StepsService = new StepsService(db);
        SubStepsService = new SubStepsService(db);
        DungeonsService = new DungeonsService(db);
        QuestsService = new QuestsService(db);
        ItemsService = new ItemsService(db);
        ItemSubStepsService = new ItemSubStepsService(db);
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
        db.CreateTable<Monsters>();
        db.CreateTable<MonsterSubSteps>();
        db.CreateTable<Users>();

        StartCoroutine(SyncAndThenSyncGuides());
    }

    void OnDestroy()
    {
        db.Close();
    }

    // SYNCHRO
    private IEnumerator SyncAndThenSyncGuides()
    {
        yield return StartCoroutine(SyncDataFromApi("https://ganymede-dofus.com/api/sync"));
        StartSyncGuides();
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

        if (jsonObj["quests"] is JArray questsArray)
        {
            foreach (JObject quest in questsArray)
            {
                ProcessQuest(quest);
            }
        }

        if (jsonObj["monsters"] is JArray monstersArray)
        {
            foreach (JObject monster in monstersArray)
            {
                ProcessMonster(monster);
            }
        }

        if (jsonObj["dungeons"] is JArray dungeonsArray)
        {
            foreach (JObject dungeon in dungeonsArray)
            {
                ProcessDungeon(dungeon);
            }
        }

        if (jsonObj["items"] is JArray itemsArray)
        {
            foreach (JObject item in itemsArray)
            {
                ProcessItem(item);
            }
        }
    }

    public void StartSyncGuides()
    {
        StartCoroutine(SyncGuides("https://ganymede-dofus.com/api/guides"));
    }

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
                foreach (JObject guide in guidesArray)
                {
                    int guideId = (int)guide["id"];
                    StartCoroutine(getAPIGuide(guideId));
                }
            }
        }
    }

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

    private void ProcessQuest(JObject quest)
    {
        Quests newQuest = new Quests
        {
            dofusDbId = (int)quest["dofusdb_id"],
            apiId = (int)quest["id"],
            name = (string)quest["name"]
        };

        Quests existingQuest = QuestsService.GetQuestByApiId(newQuest.apiId);
        if (existingQuest == null)
        {
            db.Insert(newQuest);
        }
        else
        {
            newQuest.id = existingQuest.id;
            db.Update(newQuest);
        }
    }

    private void ProcessMonster(JObject monster)
    {
        Monsters newMonster = new Monsters
        {
            dofusDbId = (int)monster["dofusdb_id"],
            apiId = (int)monster["id"],
            name = (string)monster["name"],
            imageUrl = (string)monster["image_url"]
        };

        Monsters existingMonster = MonstersService.GetMonsterByApiId(newMonster.apiId);
        if (existingMonster == null)
        {
            db.Insert(newMonster);
        }
        else
        {
            newMonster.id = existingMonster.id;
            db.Update(newMonster);
        }
    }

    private void ProcessDungeon(JObject dungeon)
    {
        Dungeons newDungeon = new Dungeons
        {
            dofusDbId = (int)dungeon["dofusdb_id"],
            apiId = (int)dungeon["id"],
            name = (string)dungeon["name"],
            level = (int)dungeon["level"]
        };

        Dungeons existingDungeon = DungeonsService.GetDungeonByApiId(newDungeon.apiId);
        if (existingDungeon == null)
        {
            db.Insert(newDungeon);
        }
        else
        {
            newDungeon.id = existingDungeon.id;
            db.Update(newDungeon);
        }
    }

    private void ProcessItem(JObject item)
    {
        Items newItem = new Items
        {
            dofusDbId = (int)item["dofusdb_id"],
            apiId = (int)item["id"],
            name = (string)item["name"],
            imageUrl = (string)item["image_url"]
        };

        Items existingItem = ItemsService.GetItemByApiId(newItem.apiId);
        if (existingItem == null)
        {
            db.Insert(newItem);
        }
        else
        {
            newItem.id = existingItem.id;
            db.Update(newItem);
        }
    }

    // CREATION D'UN GUIDE
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
            if (existingGuide == null)
            {
                Guides guide = new Guides
                {
                    apiId = apiId,
                    name = (string)jsonObj["name"],
                    description = (string)jsonObj["description"],
                    status = (string)jsonObj["status"],
                    updatedAt = (string)jsonObj["updated_at"]

                };

                db.Insert(guide);  // Assurez-vous que l'objet Guide est inséré pour obtenir son ID

                if (jsonObj["steps"] is JArray stepsArray)
                {
                    guide.steps = new List<Steps>();
                    foreach (JObject step in stepsArray)
                    {
                        Steps newStep = new Steps
                        {
                            guideId = guide.id,  // Utilisation de l'ID de Guide après son insertion
                            apiId = (int)step["id"],
                            name = (string)step["name"],
                            position = (int)step["order"],
                            posX = (int)step["pos_x"],
                            posY = (int)step["pos_y"],
                            updatedAt = (string)step["updated_at"]
                        };

                        db.Insert(newStep);  // Insertion pour obtenir l'ID de Step

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

                                db.Insert(newSubStep);  // Insertion pour obtenir l'ID de SubStep

                                // Traitez ici les liens vers des objets spécifiques comme Items ou Monsters
                                // Par exemple, pour des Items:
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
                Debug.Log("Guide already exists. Details:");

                // Charger les relations du guide existant
                GuidesService.LoadRelations(existingGuide);

                // Afficher les détails du guide
                Debug.Log($"Guide: {existingGuide.name}, Description: {existingGuide.description}, Status: {existingGuide.status}, Updated At: {existingGuide.updatedAt}");
                foreach (var step in existingGuide.steps)
                {
                    Debug.Log($"  Step: {step.name}, Position: {step.position}, Updated At: {step.updatedAt}");
                    foreach (var subStep in step.subSteps)
                    {
                        Debug.Log($"    SubStep: {subStep.type}, Text: {subStep.text}, Position: {subStep.position}, Updated At: {subStep.updatedAt}");

                        if (subStep.itemSubSteps != null)
                        {
                            foreach (var itemSubStep in subStep.itemSubSteps)
                            {
                                var item = ItemsService.GetItemById(itemSubStep.itemId);
                                Debug.Log($"      Item: {item.name}, Quantity: {itemSubStep.quantity}, Updated At: {itemSubStep.updatedAt}");
                            }
                        }

                        if (subStep.monsterSubSteps != null)
                        {
                            foreach (var monsterSubStep in subStep.monsterSubSteps)
                            {
                                var monster = MonstersService.GetMonsterById(monsterSubStep.monsterId);
                                Debug.Log($"      Monster: {monster.name}, Quantity: {monsterSubStep.quantity}, Updated At: {monsterSubStep.updatedAt}");
                            }
                        }

                        if (subStep.dungeonContent != null)
                        {
                            Debug.Log($"      Dungeon: {subStep.dungeonContent.name}, Level: {subStep.dungeonContent.level}");
                        }

                        if (subStep.questContent != null)
                        {
                            Debug.Log($"      Quest: {subStep.questContent.name}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading JSON: " + ex.Message);
        }
    }

    private void ProcessItems(JArray itemsArray, SubSteps newSubStep)
    {
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
                updatedAt = (string)item["pivot"]["updated_at"]
            };

            db.Insert(newItemSubStep); // Insérer les itemSubSteps dans la base de données
            newSubStep.itemSubSteps.Add(newItemSubStep);
        }
    }


    private void ProcessMonsters(JArray monstersArray, SubSteps newSubStep)
    {
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
                updatedAt = (string)monster["pivot"]["updated_at"]
            };

            db.Insert(newMonsterSubStep); // Insérer les MonsterSubStep dans la base de données
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
                db.Update(newSubStep);  // Mettre à jour SubSteps après l'insertion de Dungeon
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
                db.Update(newSubStep);  // Mettre à jour SubSteps après l'insertion de Quest
            }
        }
    }
}
