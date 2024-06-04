using SQLite;
using System;
using System.Collections.Generic;

public class Guides
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int apiId { get; set; }

    [MaxLength(255)]
    public string name { get; set; }
    public string description { get; set; }

    [MaxLength(10)]
    public string status { get; set; }


    public DateTime updatedAt { get; set; }

    [Ignore]
    public List<Steps> steps { get; set; }

    public override string ToString()
    {
        return $"Guide Name: {name}, Status: {status}, Updated At: {updatedAt}, Steps Count: {steps?.Count ?? 0}";
    }
}
