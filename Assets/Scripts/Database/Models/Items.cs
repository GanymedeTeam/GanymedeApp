using SQLite;
using System;
using System.Collections.Generic;

public class Items
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int dofusDbId { get; set; }
    public int apiId { get; set; }
    public string name { get; set; }
    public string imageUrl { get; set; }

    [Ignore]
    public virtual List<ItemSubSteps> itemSubSteps { get; set; }
}
