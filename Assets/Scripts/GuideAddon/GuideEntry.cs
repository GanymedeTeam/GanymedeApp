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
}

[Serializable]
public class StepEntry
{
    public string name;
    public string map;
    public int pos_x;
    public int pos_y;
    public string text;
}

[Serializable]
public class GuideEntry
{
    public int id;
    public string name;
    public string description;
    public string status;
    public int user_id;
    public string updated_at;
    public string lang;
    public List<StepEntry> steps;
}
