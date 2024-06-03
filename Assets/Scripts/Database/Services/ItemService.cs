using SQLite;
using System.Collections.Generic;

public class ItemService
{
    private SQLiteConnection db;

    public ItemService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateItem(Item item)
    {
        db.Insert(item);
    }

    public Item GetItem(int id)
    {
        return db.Table<Item>().FirstOrDefault(i => i.Id == id);
    }

    public void UpdateItem(Item item)
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

    public List<Item> GetAllItems()
    {
        return db.Table<Item>().ToList();
    }
}
