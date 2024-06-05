using SQLite;
using System;

public class ItemSubSteps
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int apiId { get; set; }
    public int quantity { get; set; }
    public int subStepId { get; set; }
    public int itemId { get; set; }
    public string updatedAt { get; set; }

    [Ignore]
    public virtual Items item { get; set; }

    [Ignore]
    public virtual SubSteps subStep { get; set; }
}
