using SQLite;
using System;
using System.Collections.Generic;

public class Monsters
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int dofusDbId { get; set; }
    public int apiId { get; set; }
    public string name { get; set; }
    public string imageUrl { get; set; }

    [Ignore]
    public virtual List<MonsterSubSteps> monsterSubSteps { get; set; }
}
