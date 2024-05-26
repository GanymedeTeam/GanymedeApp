using System;

[Serializable]
public class TravelEntry
{
    public string travelPositionName;
    public string travelPosition;
    public TravelEntry(string travelPositionName, string travelPosition)
    {
        this.travelPositionName = travelPositionName;
        this.travelPosition = travelPosition;
    }
}
