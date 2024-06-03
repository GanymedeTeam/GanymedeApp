using SQLite;
using System;

public class ItemSubSteps
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int resourceId { get; set; }
    public int apiId { get; set; }

    public int quantity { get; set; }

    public int subStepId { get; set; }

    public DateTime updatedAt { get; set; }

    [Ignore]
    public Items items { get; set; }
}
