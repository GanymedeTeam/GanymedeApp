using SQLite;
using System.Collections.Generic;

public class MonsterSubStepService
{
    private SQLiteConnection db;

    public MonsterSubStepService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateMonsterSubStep(MonsterSubStep monsterSubStep)
    {
        db.Insert(monsterSubStep);
    }

    public MonsterSubStep GetMonsterSubStep(int id)
    {
        return db.Table<MonsterSubStep>().FirstOrDefault(mss => mss.Id == id);
    }

    public void UpdateMonsterSubStep(MonsterSubStep monsterSubStep)
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

    public List<MonsterSubStep> GetAllMonsterSubSteps()
    {
        return db.Table<MonsterSubStep>().ToList();
    }
}
