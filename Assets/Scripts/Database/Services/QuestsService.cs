using SQLite;
using System.Collections.Generic;

public class QuestsService
{
    private SQLiteConnection db;

    public QuestsService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateQuest(Quests quest)
    {
        db.Insert(quest);
    }

    public Quests GetQuest(int id)
    {
        return db.Table<Quests>().FirstOrDefault(q => q.id == id);
    }

    public void UpdateQuest(Quests quest)
    {
        db.Update(quest);
    }

    public void DeleteQuest(int id)
    {
        var quest = GetQuest(id);
        if (quest != null)
        {
            db.Delete(quest);
        }
    }

    public List<Quests> GetAllQuests()
    {
        return db.Table<Quests>().ToList();
    }
}
