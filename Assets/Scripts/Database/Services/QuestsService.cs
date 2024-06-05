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

    public Quests GetQuestByApiId(int apiId)
    {
        return db.Table<Quests>().FirstOrDefault(quest => quest.apiId == apiId);
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

    public void AddOrUpdateQuest(Quests newQuest)
    {
        var existingQuest = GetQuestByApiId(newQuest.apiId);
        if (existingQuest != null)
        {
            // Mettre à jour la quest existante
            existingQuest.dofusDbId = newQuest.dofusDbId;
            existingQuest.name = newQuest.name;
            db.Update(existingQuest);
        }
        else
        {
            // Insérer la nouvelle quest
            db.Insert(newQuest);
        }
    }
}
