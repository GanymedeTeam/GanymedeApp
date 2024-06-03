using SQLite;
using System.Collections.Generic;

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

    public Monsters GetMonster(int id)
    {
        return db.Table<Monsters>().FirstOrDefault(m => m.id == id);
    }

    public void UpdateMonster(Monsters monster)
    {
        db.Update(monster);
    }

    public void DeleteMonster(int id)
    {
        var monster = GetMonster(id);
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
