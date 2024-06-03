using SQLite;
using System;
using System.Collections.Generic;

public class Steps
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    [Indexed]
    public int guideId { get; set; }

    public int apiId { get; set; }

    [MaxLength(255)]
    public string name { get; set; }

    public int position { get; set; }

    public int posX { get; set; }

    public int posY { get; set; }

    public DateTime updatedAt { get; set; }

    [Ignore]
    public List<SubSteps> subSteps { get; set; }
}
