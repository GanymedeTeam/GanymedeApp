using SQLite;
using System;

public class ItemSubStep
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int ResourceId { get; set; }

    public int Quantity { get; set; }

    public int SubStepId { get; set; }

    public DateTime UpdatedAt { get; set; }

    [Ignore]
    public Item Item { get; set; }
}
