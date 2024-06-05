using SQLite;
using System.Collections.Generic;

public class NpcsService
{
    private SQLiteConnection db;

    public NpcsService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateNpc(Npcs npc)
    {
        db.Insert(npc);
    }

    public Npcs GetNpc(int id)
    {
        return db.Table<Npcs>().FirstOrDefault(npc => npc.id == id);
    }

    public Npcs GetNpcByApiId(int apiId)
    {
        return db.Table<Npcs>().FirstOrDefault(npc => npc.apiId == apiId);
    }

    public void UpdateNpc(Npcs npc)
    {
        db.Update(npc);
    }

    public void DeleteNpc(int id)
    {
        var npc = GetNpc(id);
        if (npc != null)
        {
            db.Delete(npc);
        }
    }

    public List<Npcs> GetAllNpcs()
    {
        return db.Table<Npcs>().ToList();
    }

    public void AddOrUpdateNpc(Npcs newNpc)
    {
        var existingNpc = GetNpcByApiId(newNpc.apiId);
        if (existingNpc != null)
        {
            // Mettre à jour le npc existant
            existingNpc.dofusDbId = newNpc.dofusDbId;
            existingNpc.name = newNpc.name;
            db.Update(existingNpc);
        }
        else
        {
            // Insérer le nouveau npc
            db.Insert(newNpc);
        }
    }
}
