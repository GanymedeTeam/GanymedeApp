using SQLite;
using System;
using System.Collections.Generic;

public class Guide
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    [MaxLength(10)]
    public string Status { get; set; }

    public DateTime UpdatedAt { get; set; }

    [Ignore]
    public List<Step> Steps { get; set; }
}
