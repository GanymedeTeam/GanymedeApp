using SQLite;
using System;

public class Monster
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int DofusDbId { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    [MaxLength(1024)]
    public string ImageUrl { get; set; }

    public DateTime UpdatedAt { get; set; }
}
