using SQLite;
using System;
using System.Collections.Generic;

public class Guides
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int apiId { get; set; }
    public int userId { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public string updatedAt { get; set; }

    [Ignore]
    public virtual Users user { get; set; }

    [Ignore]
    public virtual List<Steps> steps { get; set; }
}
