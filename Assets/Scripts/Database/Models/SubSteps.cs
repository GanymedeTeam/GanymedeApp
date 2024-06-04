using SQLite;
using System;
using System.Collections.Generic;

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

    public int? contentId { get; set; }

    [MaxLength(1024)]
    public string text { get; set; }

    public DateTime updatedAt { get; set; }

    [Ignore]
    public List<ItemSubSteps> itemSubSteps { get; set; }  // Liste des ItemSubSteps

    [Ignore]
    public List<MonsterSubSteps> monsterSubSteps { get; set; }  // Liste des ItemSubSteps

    [Ignore]
    public object content { get; set; } // Utiliser 'object' pour un contenu polymorphique ou définir des types spécifiques

    [Ignore]
    public Dungeons dungeonContent { get; set; }

    [Ignore]
    public Quests questContent { get; set; }
}
