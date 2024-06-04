using SQLite;
using System;
using System.Linq;
using System.IO;
using UnityEngine;
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
    }

    void OnDestroy()
    {
        db.Close();
    }

    public void LoadDataFromJson(string jsonPath)
    {
        Debug.Log("Chemin du fichier JSON : " + jsonPath);

        try
        {
            string json = File.ReadAllText(jsonPath);
            JObject jsonObj = JObject.Parse(json);

            if (jsonObj == null)
            {
                Debug.LogError("Failed to parse JSON or JSON is empty");
                return;
            }

            int apiId = (int)jsonObj["id"];
            // DateTime updatedAtJson = DateTime.Parse((string)jsonObj["updated_at"], null, System.Globalization.DateTimeStyles.RoundtripKind);

            // Vérifiez si le guide existe déjà
            Guides existingGuide = GuidesService.GetGuideByApiId(apiId);
            if (existingGuide == null)
            {
                Guides guide = new Guides
                {
                    apiId = (int)jsonObj["id"],
                    name = (string)jsonObj["name"],
                    description = (string)jsonObj["description"] != null ? (string)jsonObj["description"] : null,
                    status = (string)jsonObj["status"],
                    updatedAt = DateTime.UtcNow
                };

                if (jsonObj["steps"] is JArray stepsArray)
                {
                    guide.steps = stepsArray.Select(step =>
                    {
                        JObject s = (JObject)step;
                        Steps newStep = new Steps
                        {
                            guideId = guide.id,
                            apiId = (int)s["id"],
                            name = (string)s["name"],
                            position = (int)s["order"],
                            posX = (int)s["pos_x"],
                            posY = (int)s["pos_y"],
                            updatedAt = DateTime.UtcNow
                        };

                        if (s["sub_steps"] is JArray subStepsArray)
                        {
                            newStep.subSteps = subStepsArray.Select(sub_step =>
                            {
                                var ss = (JObject)sub_step;
                                Debug.Log("Processing SubStep ss: " + ss);
                                var newSubStep = new SubSteps
                                {
                                    type = (string)ss["type"],
                                    stepId = newStep.id,
                                    apiId = (int)ss["id"],
                                    position = (int)ss["order"],
                                    contentType = ss["content_type"] != null ? (string)ss["content_type"] : null,
                                    contentId = ss["content_id"] != null ? (int?)ss["content_id"] : (int?)null,
                                    text = ss["text"] != null ? (string)ss["text"] : null,
                                    updatedAt = DateTime.UtcNow
                                };

                                Debug.Log("Processing SubStep: " + newSubStep.type);

                                if ((string)ss["type"] == "item")
                                {
                                    if (ss["items"] is JArray itemsArray)
                                    {
                                        newSubStep.itemSubSteps = itemsArray.Select(item =>
                                        {
                                            // Création de l'objet Item
                                            var newItem = new Items {
                                                dofusDbId = (int)item["dofusdb_id"],
                                                apiId = (int)item["id"],
                                                name = (string)item["name"],
                                                imageUrl = (string)item["image_url"],
                                                updatedAt = DateTime.UtcNow
                                            };

                                            // Enregistrement de l'objet Item dans la base de données si il existe pas
                                            ItemsService.AddOrUpdateItem(newItem);

                                            var it = (JObject)item;
                                            return new ItemSubSteps
                                            {
                                                subStepId = newSubStep.id,
                                                resourceId = (int)it["pivot"]["item_id"],
                                                apiId = (int)it["pivot"]["sub_step_id"],
                                                quantity = (int)it["pivot"]["quantity"],
                                                updatedAt = DateTime.UtcNow
                                            };
                                        }).ToList();
                                    }
                                }
                                else if ((string)ss["type"] == "monster")
                                {
                                    if (ss["monsters"] is JArray monstersArray)
                                    {
                                        newSubStep.monsterSubSteps = monstersArray.Select(monster =>
                                        {
                                            var newMonster = new Monsters {
                                                dofusDbId = (int)monster["dofusdb_id"],
                                                apiId = (int)monster["id"],
                                                name = (string)monster["name"],
                                                imageUrl = (string)monster["image_url"],
                                                updatedAt = DateTime.UtcNow
                                            };
                                            
                                            // Enregistrement de l'objet Item dans la base de données
                                            db.Insert(newMonster);

                                            var mo = (JObject)monster;
                                            return new MonsterSubSteps
                                            {
                                                subStepId = newSubStep.id,
                                                monsterId = (int)mo["pivot"]["monster_id"],
                                                apiId = (int)mo["pivot"]["sub_step_id"],
                                                quantity = (int)mo["pivot"]["quantity"],
                                                updatedAt = DateTime.UtcNow
                                            };
                                        }).ToList();
                                    }
                                }
                                else if ((string)ss["type"] == "dungeon" || (string)ss["type"] == "quest")
                                {
                                    if (ss["content"] is JObject content)
                                    {
                                        if ((string)ss["type"] == "dungeon")
                                        {
                                            newSubStep.content = new Dungeons
                                            {
                                                dofusDbId = (int)content["dofusdb_id"],
                                                apiId = (int)content["id"],
                                                name = (string)content["name"],
                                                level = (int)content["level"],
                                                updatedAt = DateTime.UtcNow
                                            };
                                        }
                                        else if ((string)ss["type"] == "quest")
                                        {
                                            newSubStep.content = new Quests
                                            {
                                                dofusDbId = (int)content["dofusdb_id"],
                                                apiId = (int)content["id"],
                                                name = (string)content["name"],
                                                updatedAt = DateTime.UtcNow
                                            };
                                        }
                                    }
                                }

                                return newSubStep;
                            }).ToList();
                        }

                        return newStep;
                    }).ToList();
                }
                GuidesService.AddGuide(guide);
                Debug.Log("Guide loaded and saved successfully.");
            }
            else
            {
                Debug.Log("Guide is in database");
            }
        } 

        catch (Exception ex)
        {
            Debug.LogError("Error loading JSON: " + ex.Message);
        }
    }
}
