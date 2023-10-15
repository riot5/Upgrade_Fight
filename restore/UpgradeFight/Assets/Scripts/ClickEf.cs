using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ClickEf : MonoBehaviour, IPointerDownHandler
{

    public Camera cam;
    public GameObject clickEffect;

    RaycastHit hit;

    public void OnPointerDown(PointerEventData eventData)
    {
        Ray ray = cam.ScreenPointToRay(eventData.position);
        Physics.Raycast(ray, out hit);
        clickEffect.transform.position = hit.point;
    }
}
