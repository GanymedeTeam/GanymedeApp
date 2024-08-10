using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuDropdown : MonoBehaviour
{
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    void Start()
    {
        // Obtenir le GraphicRaycaster et l'EventSystem
        graphicRaycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        if (graphicRaycaster == null)
        {
            Debug.LogError("GraphicRaycaster not found. Ensure there is a Canvas with a GraphicRaycaster component in the parent hierarchy.");
        }

        if (eventSystem == null)
        {
            Debug.LogError("EventSystem not found. Ensure there is an EventSystem in the scene.");
        }
    }

    void Update()
    {
        // Détecter un clic de la souris
        if (Input.GetMouseButtonDown(0))
        {
            // Effectuer un raycast pour les éléments UI
            PointerEventData pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            bool clickedOnTarget = false;

            foreach (RaycastResult result in results)
            {
                // Vérifier si le gameObject touché est lui-même ou un de ses enfants
                if (result.gameObject.transform.IsChildOf(transform))
                {
                    clickedOnTarget = true;
                    break;
                }
            }

            // Si aucun élément UI correspondant n'est cliqué, désactiver ce gameObject
            if (!clickedOnTarget)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnDisable()
    {
        FindObjectOfType<WindowManager>().EnableDropdown();
    }
}
