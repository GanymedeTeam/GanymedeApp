using SQLite;
using System.Collections.Generic;

public class GuidesService
{
    private SQLiteConnection db;

    public GuidesService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateGuide(Guides guide)
    {
        db.Insert(guide);
    }

    public Guides GetGuide(int id)
    {
        return db.Table<Guides>().FirstOrDefault(g => g.id == id);
    }

    public void UpdateGuide(Guides guide)
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

    public List<Guides> GetAllGuides()
    {
        return db.Table<Guides>().ToList();
    }
}
