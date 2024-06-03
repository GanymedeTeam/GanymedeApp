using SQLite;
using System.Collections.Generic;

public class SubStepService
{
    private SQLiteConnection db;

    public SubStepService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateSubStep(SubStep subStep)
    {
        db.Insert(subStep);
    }

    public SubStep GetSubStep(int id)
    {
        return db.Table<SubStep>().FirstOrDefault(ss => ss.Id == id);
    }

    public void UpdateSubStep(SubStep subStep)
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

    public List<SubStep> GetAllSubSteps()
    {
        return db.Table<SubStep>().ToList();
    }
}
