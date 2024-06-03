using SQLite;
using System.Collections.Generic;

public class SubStepsService
{
    private SQLiteConnection db;

    public SubStepsService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateSubStep(SubSteps subStep)
    {
        db.Insert(subStep);
    }

    public SubSteps GetSubStep(int id)
    {
        return db.Table<SubSteps>().FirstOrDefault(ss => ss.id == id);
    }

    public void UpdateSubStep(SubSteps subStep)
    {
        db.Update(subStep);
    }

    public void DeleteSubStep(int id)
    {
        var subStep = GetSubStep(id);
        if (subStep != null)
        {
            db.Delete(subStep);
        }
    }

    public List<SubSteps> GetAllSubSteps()
    {
        return db.Table<SubSteps>().ToList();
    }
}
