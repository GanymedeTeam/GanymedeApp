using SQLite;
using System.Collections.Generic;

public class StepsService
{
    private SQLiteConnection db;

    public StepsService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateStep(Steps step)
    {
        db.Insert(step);
    }

    public Steps GetStep(int id)
    {
        return db.Table<Steps>().FirstOrDefault(s => s.id == id);
    }

    public Steps GetStepByApiId(int apiId)
    {
        return db.Table<Steps>().FirstOrDefault(s => s.apiId == apiId);
    }

    public void UpdateStep(Steps step)
    {
        db.Update(step);
    }

    public void DeleteStep(int id)
    {
        var step = GetStep(id);
        if (step != null)
        {
            db.Delete(step);
        }
    }

    public List<Steps> GetAllSteps()
    {
        return db.Table<Steps>().ToList();
    }

    public List<Steps> GetStepsByGuideId(int guideId)
    {
        return db.Table<Steps>().Where(s => s.guideId == guideId).ToList();
    }
}
