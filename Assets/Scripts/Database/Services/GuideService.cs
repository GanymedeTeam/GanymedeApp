using SQLite;
using System.Collections.Generic;

public class GuideService
{
    private SQLiteConnection db;

    public GuideService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateGuide(Guide guide)
    {
        db.Insert(guide);
    }

    public Guide GetGuide(int id)
    {
        return db.Table<Guide>().FirstOrDefault(g => g.Id == id);
    }

    public void UpdateGuide(Guide guide)
    {
        db.Update(guide);
    }

    public void DeleteGuide(int id)
    {
        var guide = GetGuide(id);
        if (guide != null)
        {
            db.Delete(guide);
        }
    }

    public List<Guide> GetAllGuides()
    {
        return db.Table<Guide>().ToList();
    }
}
