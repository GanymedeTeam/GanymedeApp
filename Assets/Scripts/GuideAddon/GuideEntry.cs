using System;
using System.Collections.Generic;

[Serializable]
public class ContentEntry
{
    public string name;
    public int dofusdb_id;
    public string image_url;
    public int quantity;
    public int level;

    public ContentEntry(
        string name,
        int dofusdb_id,
        string image_url,
        int quantity,
        int level
    )
    {
        this.name = name;
        this.dofusdb_id = dofusdb_id;
        this.image_url = image_url;
        this.quantity = quantity;
        this.level = level;
    }
}

[Serializable]
public class SubstepEntry
{
    public string type;
    public string text;
    public List<ContentEntry> content;
    
    public void SubStepEntry(
        string type,
        string text,
        List<ContentEntry> content
    )
    {
        this.type = type;
        this.text = text;
        this.content = content;
    }
}

[Serializable]
public class StepEntry
{
    public string name;
    public string map;
    public int pos_x;
    public int pos_y;
    public List<SubstepEntry> sub_steps;

    public StepEntry(
        string name,
        string map,
        int pos_x,
        int pos_y,
        List<SubstepEntry> sub_steps
    )
    {
        this.name = name;
        this.map = map;
        this.pos_x = pos_x;
        this.pos_y = pos_y;
        this.sub_steps = sub_steps;
    }
}

[Serializable]
public class GuideEntry
{
    public int id;
    public string name;
    public string description;
    public string status;
    public int user_id;
    public List<StepEntry> steps;

    public GuideEntry(
        int id,
        string name,
        string description,
        string status,
        int user_id,
        List<StepEntry> steps
    )
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.status = status;
        this.user_id = user_id;
        this.steps = steps;
    }
}
