using SQLite;
using System;
using System.Collections.Generic;

public class Dungeons
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int dofusDbId { get; set; }
    public int apiId { get; set; }
    public string name { get; set; }
    public int level { get; set; }

    [Ignore]
    public virtual List<SubSteps> subSteps { get; set; }
}
