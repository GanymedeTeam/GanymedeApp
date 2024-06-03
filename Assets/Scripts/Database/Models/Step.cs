using SQLite;
using System;
using System.Collections.Generic;

public class Step
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int GuideId { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    public int Position { get; set; }

    public int PosX { get; set; }

    public int PosY { get; set; }

    public DateTime UpdatedAt { get; set; }

    [Ignore]
    public List<SubStep> SubSteps { get; set; }
}
