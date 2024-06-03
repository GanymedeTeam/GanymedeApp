using SQLite;
using System.Collections.Generic;

public class MonsterSubStepsService
{
    private SQLiteConnection db;

    public MonsterSubStepsService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateMonsterSubStep(MonsterSubSteps monsterSubStep)
    {
        db.Insert(monsterSubStep);
    }

    public MonsterSubSteps GetMonsterSubStep(int id)
    {
        return db.Table<MonsterSubSteps>().FirstOrDefault(mss => mss.id == id);
    }

    public void UpdateMonsterSubStep(MonsterSubSteps monsterSubStep)
    {
        db.Update(monsterSubStep);
    }

    public void DeleteMonsterSubStep(int id)
    {
        var monsterSubStep = GetMonsterSubStep(id);
        if (monsterSubStep != null)
        {
            db.Delete(monsterSubStep);
        }
    }

    public List<MonsterSubSteps> GetAllMonsterSubSteps()
    {
        return db.Table<MonsterSubSteps>().ToList();
    }
}
