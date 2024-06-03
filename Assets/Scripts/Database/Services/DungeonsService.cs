using SQLite;
using System.Collections.Generic;

public class DungeonsService
{
    private SQLiteConnection db;

    public DungeonsService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateDungeon(Dungeons dungeon)
    {
        db.Insert(dungeon);
    }

    public Dungeons GetDungeon(int id)
    {
        return db.Table<Dungeons>().FirstOrDefault(d => d.id == id);
    }

    public void UpdateDungeon(Dungeons dungeon)
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

    public List<Dungeons> GetAllDungeons()
    {
        return db.Table<Dungeons>().ToList();
    }
}
