using SQLite;
using System.Collections.Generic;

public class ItemSubStepService
{
    private SQLiteConnection db;

    public ItemSubStepService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateItemSubStep(ItemSubStep itemSubStep)
    {
        db.Insert(itemSubStep);
    }

    public ItemSubStep GetItemSubStep(int id)
    {
        return db.Table<ItemSubStep>().FirstOrDefault(iss => iss.Id == id);
    }

    public void UpdateItemSubStep(ItemSubStep itemSubStep)
    {
        db.Update(itemSubStep);
    }

    public void DeleteItemSubStep(int id)
    {
        var itemSubStep = GetItemSubStep(id);
        if (itemSubStep != null)
        {
            db.Delete(itemSubStep);
        }
    }

    public List<ItemSubStep> GetAllItemSubSteps()
    {
        return db.Table<ItemSubStep>().ToList();
    }
}
