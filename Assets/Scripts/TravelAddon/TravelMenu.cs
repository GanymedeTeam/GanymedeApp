using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TravelMenu : MonoBehaviour
{

    public TMP_Text positionNameText;
    public TMP_Text positionText;
    public GameObject travelUIPositionPrefab;
    public GameObject gridGameobject;

    private IEnumerator ResizeContentGridLayout()
    {
        yield return 0;
        gridGameobject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(gridGameobject.GetComponent<RectTransform>().rect.width - 15f, 32f);
    }

    public void OnEnable()
    {
        StartCoroutine(ResizeContentGridLayout());
    }

    public List<TravelEntry> travelPositionsData = new List<TravelEntry>();
    // Start is called before the first frame update
    void Start()
    {
        travelPositionsData = FileHandler.ReadListFromJSON<TravelEntry>("travel/travelPositions.json");

        foreach (TravelEntry travelEntry in travelPositionsData)
        {
            GameObject newTravelPosition = Instantiate(travelUIPositionPrefab, gridGameobject.transform);
            TravelPositionObject travelPositionObject = newTravelPosition.GetComponent<TravelPositionObject>();
            travelPositionObject.Initialize(travelEntry.travelPositionName, travelEntry.travelPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTravelPosition()
    {
        if(positionNameText.text.Length > 1 && positionText.text.Length > 1)
        {
            GameObject newTravelPosition = Instantiate(travelUIPositionPrefab, gridGameobject.transform);    
            TravelPositionObject travelPositionObject = newTravelPosition.GetComponent<TravelPositionObject>();    
            travelPositionObject.Initialize(positionNameText.text, positionText.text);    
            travelPositionsData.Add(new TravelEntry(positionNameText.text,positionText.text));    
            positionNameText.text = "";
            positionText.text = "";
            FileHandler.SaveToJSON(travelPositionsData, "travel/travelPositions.json");
        }
    }
}
