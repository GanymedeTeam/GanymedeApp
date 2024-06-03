using SQLite;
using System.IO;
using UnityEngine;

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
}
