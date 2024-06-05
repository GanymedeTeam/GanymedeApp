using SQLite;
using System.Collections.Generic;
using System.Linq;

public class MonstersService
{
    private SQLiteConnection db;

    public MonstersService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateMonster(Monsters monster)
    {
        db.Insert(monster);
    }

    public Monsters GetMonsterByApiId(int apiId)
    {
        return db.Table<Monsters>().FirstOrDefault(m => m.apiId == apiId);
    }

    public Monsters GetMonsterById(int id)
    {
        return db.Find<Monsters>(id);
    }

    public void UpdateMonster(Monsters monster)
    {
        db.Update(monster);
    }

    public void DeleteMonster(int id)
    {
        var monster = GetMonsterById(id);
        if (monster != null)
        {
            db.Delete(monster);
        }
    }

    public List<Monsters> GetAllMonsters()
    {
        return db.Table<Monsters>().ToList();
    }
}
