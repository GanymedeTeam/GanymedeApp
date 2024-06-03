using SQLite;
using System;

public class Quest
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int DofusDbId { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    public DateTime UpdatedAt { get; set; }
}
