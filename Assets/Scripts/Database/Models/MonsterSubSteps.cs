using SQLite;
using System;

public class MonsterSubSteps
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int apiId { get; set; }
    public int quantity { get; set; }
    public int subStepId { get; set; }
    public int monsterId { get; set; }
    public string updatedAt { get; set; }

    [Ignore]
    public virtual Monsters monster { get; set; }

    [Ignore]
    public virtual SubSteps subStep { get; set; }
}
