using SQLite;
using System.Collections.Generic;

public class StepService
{
    private SQLiteConnection db;

    public StepService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateStep(Step step)
    {
        db.Insert(step);
    }

    public Step GetStep(int id)
    {
        return db.Table<Step>().FirstOrDefault(s => s.Id == id);
    }

    public void UpdateStep(Step step)
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

    public List<Step> GetAllSteps()
    {
        return db.Table<Step>().ToList();
    }
}
