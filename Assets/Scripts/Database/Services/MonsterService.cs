using SQLite;
using System.Collections.Generic;

public class MonsterService
{
    private SQLiteConnection db;

    public MonsterService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateMonster(Monster monster)
    {
        db.Insert(monster);
    }

    public Monster GetMonster(int id)
    {
        return db.Table<Monster>().FirstOrDefault(m => m.Id == id);
    }

    public void UpdateMonster(Monster monster)
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

    public List<Monster> GetAllMonsters()
    {
        return db.Table<Monster>().ToList();
    }
}
