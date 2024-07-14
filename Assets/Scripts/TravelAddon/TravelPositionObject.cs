using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class TravelPositionObject : MonoBehaviour
{
    public TMP_Text travelPositionNameText;
    public string travelPosition;
    public string travelPositionName;
    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = (Convert.ToBoolean(PlayerPrefs.GetInt("wantTravel", 1)) ? "/travel " : "") + travelPosition;
    }

    public void Initialize(string travelPositionName, string travelPosition)
    {
        this.travelPositionName = travelPositionName;
        this.travelPosition = travelPosition;
        travelPositionNameText.text = travelPositionName;
    }

    public void RemovePosition()
    {
        TravelMenu travelMenu = FindObjectOfType<TravelMenu>();
        travelMenu.travelPositionsData.Remove(travelMenu.travelPositionsData.Find(x => x.travelPosition == travelPosition));
        FileHandler.SaveToJSON(travelMenu.travelPositionsData, "travel/travelPositions.json");
        Destroy(gameObject);
    }

}