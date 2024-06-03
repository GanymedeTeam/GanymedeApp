using SQLite;
using System.IO;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private SQLiteConnection db;
    public GuideService GuideService { get; private set; }
    public StepService StepService { get; private set; }
    public SubStepService SubStepService { get; private set; }
    public DungeonService DungeonService { get; private set; }
    public QuestService QuestService { get; private set; }
    public ItemService ItemService { get; private set; }
    public ItemSubStepService ItemSubStepService { get; private set; }
    public MonsterService MonsterService { get; private set; }
    public MonsterSubStepService MonsterSubStepService { get; private set; }
    public UserService UserService { get; private set; }

    void Awake()
    {
        string databasePath = Path.Combine(Application.persistentDataPath, "MyDatabase.db");
        db = new SQLiteConnection(databasePath);

        // Initialize services
        GuideService = new GuideService(db);
        StepService = new StepService(db);
        SubStepService = new SubStepService(db);
        DungeonService = new DungeonService(db);
        QuestService = new QuestService(db);
        ItemService = new ItemService(db);
        ItemSubStepService = new ItemSubStepService(db);
        MonsterService = new MonsterService(db);
        MonsterSubStepService = new MonsterSubStepService(db);
        UserService = new UserService(db);

        // Create tables if they don't exist
        db.CreateTable<Guide>();
        db.CreateTable<Step>();
        db.CreateTable<SubStep>();
        db.CreateTable<Dungeon>();
        db.CreateTable<Quest>();
        db.CreateTable<Item>();
        db.CreateTable<ItemSubStep>();
        db.CreateTable<Monster>();
        db.CreateTable<MonsterSubStep>();
        db.CreateTable<User>();
    }

    void OnDestroy()
    {
        db.Close();
    }
}
