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

    public Items GetItemByApiId(int apiId)
    {
        return db.Table<Items>().FirstOrDefault(i => i.apiId == apiId);
    }

    public Items GetItemById(int id)
    {
        return db.Find<Items>(id);
    }

    public void UpdateItem(Items item)
    {
        db.Update(item);
    }

    public void DeleteItem(int id)
    {
        var item = GetItemById(id);
        if (item != null)
        {
            db.Delete(item);
        }
    }

    public List<Items> GetAllItems()
    {
        return db.Table<Items>().ToList();
    }
}
