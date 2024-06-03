using SQLite;
using System;

public class SubSteps
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    [MaxLength(10)]
    public string type { get; set; }

    [Indexed]
    public int stepId { get; set; }

    public int apiId { get; set; }

    public int position { get; set; }

    [MaxLength(10)]
    public string contentType { get; set; }

    public int contentId { get; set; }

    [MaxLength(1024)]
    public string text { get; set; }

    public DateTime updatedAt { get; set; }

    [Ignore]
    public object content { get; set; }  // Content could be Dungeon, Quest, SubStepItem or SubStepMonster
}
