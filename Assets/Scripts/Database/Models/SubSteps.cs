using SQLite;
using System;
using System.Collections.Generic;

public class SubSteps
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    [MaxLength(10)]
    public string type { get; set; }

    public int stepId { get; set; }
    public int apiId { get; set; }
    public int position { get; set; }
    public string text { get; set; }
    public string updatedAt { get; set; }
    // Clés étrangères pour les contenus
    public int? dungeonContentId { get; set; }
    public int? questContentId { get; set; }
    public int? npcContentId { get; set; }

    [Ignore]
    public virtual Steps step { get; set; }

    [Ignore]
    public virtual List<MonsterSubSteps> monsterSubSteps { get; set; }

    [Ignore]
    public virtual List<ItemSubSteps> itemSubSteps { get; set; }

    // Propriétés de navigation pour les contents
    [Ignore]
    public virtual Dungeons dungeonContent { get; set; }

    [Ignore]
    public virtual Quests questContent { get; set; }

    [Ignore]
    public virtual Npcs npcContent { get; set; }
}
