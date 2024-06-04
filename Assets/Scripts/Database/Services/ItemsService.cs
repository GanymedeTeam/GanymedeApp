using SQLite;
using System.Collections.Generic;

public class ItemsService
{
    private SQLiteConnection db;

    public ItemsService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateItem(Items item)
    {
        db.Insert(item);
    }

    public Items GetItem(int id)
    {
        return db.Table<Items>().FirstOrDefault(i => i.id == id);
    }

    public Items GetItemByApiId(int apiId)
    {
        return db.Table<Items>().FirstOrDefault(item => item.apiId == apiId);
    }

    public void UpdateItem(Items item)
    {
        db.Update(item);
    }

    public void DeleteItem(int id)
    {
        var item = GetItem(id);
        if (item != null)
        {
            db.Delete(item);
        }
    }

    public List<Items> GetAllItems()
    {
        return db.Table<Items>().ToList();
    }

    public void AddOrUpdateItem(Items newItem)
    {
        var existingItem = GetItemByApiId(newItem.apiId);
        if (existingItem != null)
        {
            // Mettre à jour l'item existant
            existingItem.dofusDbId = newItem.dofusDbId;
            existingItem.name = newItem.name;
            existingItem.imageUrl = newItem.imageUrl;
            existingItem.updatedAt = newItem.updatedAt;
            db.Update(existingItem);
        }
        else
        {
            // Insérer le nouvel item
            db.Insert(newItem);
        }
    }
}
