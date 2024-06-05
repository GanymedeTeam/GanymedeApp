using SQLite;
using System.Collections.Generic;
using System.Linq;

public class ItemSubStepsService
{
    private SQLiteConnection db;

    public ItemSubStepsService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateItemSubStep(ItemSubSteps itemSubStep)
    {
        db.Insert(itemSubStep);
    }

    public ItemSubSteps GetItemSubStep(int id)
    {
        return db.Table<ItemSubSteps>().FirstOrDefault(iss => iss.id == id);
    }

    public ItemSubSteps GetItemSubStepByApiId(int apiId)
    {
        return db.Table<ItemSubSteps>().FirstOrDefault(iss => iss.apiId == apiId);
    }

    public void UpdateItemSubStep(ItemSubSteps itemSubStep)
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

    public List<ItemSubSteps> GetAllItemSubSteps()
    {
        return db.Table<ItemSubSteps>().ToList();
    }

    public List<ItemSubSteps> GetItemSubStepsBySubStepId(int subStepId)
    {
        return db.Table<ItemSubSteps>().Where(iss => iss.subStepId == subStepId).ToList();
    }
}
