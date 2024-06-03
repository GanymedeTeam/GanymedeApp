using SQLite;
using System;

public class MonsterSubSteps
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int monsterId { get; set; }
    public int apiId { get; set; }

    public int quantity { get; set; }

    public int subStepId { get; set; }

    public DateTime updatedAt { get; set; }

    [Ignore]
    public Monsters monsters { get; set; }
}
