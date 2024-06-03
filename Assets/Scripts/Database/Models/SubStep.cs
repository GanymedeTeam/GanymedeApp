using SQLite;
using System;

public class SubStep
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(10)]
    public string Type { get; set; }

    [Indexed]
    public int StepId { get; set; }

    public int Position { get; set; }

    [MaxLength(10)]
    public string ContentType { get; set; }

    public int ContentId { get; set; }

    [MaxLength(1024)]
    public string Text { get; set; }

    public DateTime UpdatedAt { get; set; }

    [Ignore]
    public object Content { get; set; }  // Content could be Dungeon, Quest, SubStepItem or SubStepMonster
}
