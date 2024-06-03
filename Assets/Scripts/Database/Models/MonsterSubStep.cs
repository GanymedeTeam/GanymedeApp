using SQLite;
using System;

public class MonsterSubStep
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int MonsterId { get; set; }

    public int Quantity { get; set; }

    public int SubStepId { get; set; }

    public DateTime UpdatedAt { get; set; }

    [Ignore]
    public Monster Monster { get; set; }
}
