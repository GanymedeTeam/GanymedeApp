using SQLite;
using System.Collections.Generic;

public class QuestService
{
    private SQLiteConnection db;

    public QuestService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateQuest(Quest quest)
    {
        db.Insert(quest);
    }

    public Quest GetQuest(int id)
    {
        return db.Table<Quest>().FirstOrDefault(q => q.Id == id);
    }

    public void UpdateQuest(Quest quest)
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

    public List<Quest> GetAllQuests()
    {
        return db.Table<Quest>().ToList();
    }
}
