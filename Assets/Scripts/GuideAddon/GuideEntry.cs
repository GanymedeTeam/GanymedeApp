using System;

[Serializable]
public class GuideEntry
{
    public int ID;
    public string travelPosition;
    public string title;
    public string description;
    public string map;

    public GuideEntry(int ID, string travelPosition, string title, string description, string map)
    {
        this.ID = ID;
        this.travelPosition = travelPosition;
        this.title = title;
        this.description = description;
        this.map = map;
    }
}
