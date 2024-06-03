using SQLite;
using System;

public class Dungeon
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int DofusDbId { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    public int Level { get; set; }

    public DateTime UpdatedAt { get; set; }
}
