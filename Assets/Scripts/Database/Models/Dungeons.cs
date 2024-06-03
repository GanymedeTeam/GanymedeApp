using SQLite;
using System;

public class Dungeons
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int dofusDbId { get; set; }
    public int apiId { get; set; }

    [MaxLength(255)]
    public string name { get; set; }

    public int level { get; set; }

    public DateTime updatedAt { get; set; }
}
