using SQLite;
using System;
using System.Collections.Generic;

public class Steps
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int guideId { get; set; }
    public int apiId { get; set; }
    public string name { get; set; }
    public string map { get; set; }
    public int position { get; set; }
    public int posX { get; set; }
    public int posY { get; set; }
    public string updatedAt { get; set; }

    [Ignore]
    public virtual Guides guide { get; set; }

    [Ignore]
    public virtual List<SubSteps> subSteps { get; set; }
}
