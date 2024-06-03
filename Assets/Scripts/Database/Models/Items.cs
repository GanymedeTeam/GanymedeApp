using SQLite;
using System;

public class Items
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int dofusDbId { get; set; }
    public int apiId { get; set; }

    [MaxLength(255)]
    public string name { get; set; }

    [MaxLength(1024)]
    public string imageUrl { get; set; }

    public DateTime updatedAt { get; set; }
}
