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
}
