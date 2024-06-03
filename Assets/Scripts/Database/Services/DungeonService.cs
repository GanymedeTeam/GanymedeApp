using SQLite;
using System.Collections.Generic;

public class DungeonService
{
    private SQLiteConnection db;

    public DungeonService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateDungeon(Dungeon dungeon)
    {
        db.Insert(dungeon);
    }

    public Dungeon GetDungeon(int id)
    {
        return db.Table<Dungeon>().FirstOrDefault(d => d.Id == id);
    }

    public void UpdateDungeon(Dungeon dungeon)
    {
        db.Update(dungeon);
    }

    public void DeleteDungeon(int id)
    {
        var dungeon = GetDungeon(id);
        if (dungeon != null)
        {
            db.Delete(dungeon);
        }
    }

    public List<Dungeon> GetAllDungeons()
    {
        return db.Table<Dungeon>().ToList();
    }
}
