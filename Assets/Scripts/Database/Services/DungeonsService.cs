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

    public Dungeons GetDungeonByApiId(int apiId)
    {
        return db.Table<Dungeons>().FirstOrDefault(dungeon => dungeon.apiId == apiId);
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

    public void AddOrUpdateDungeon(Dungeons newDungeon)
    {
        var existingDungeon = GetDungeonByApiId(newDungeon.apiId);
        if (existingDungeon != null)
        {
            // Mettre à jour le dungeon existant
            existingDungeon.dofusDbId = newDungeon.dofusDbId;
            existingDungeon.name = newDungeon.name;
            existingDungeon.level = newDungeon.level;
            existingDungeon.updatedAt = newDungeon.updatedAt;
            db.Update(existingDungeon);
        }
        else
        {
            // Insérer le nouveau dungeon
            db.Insert(newDungeon);
        }
    }
}
