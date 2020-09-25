using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler,IPointerUpHandler,IPointerDownHandler {

    private Image dragSpace;
    public Image joyStickImg;
    private Vector2 joyStickImagePos;
    public Vector3 joyStickVector;
    public float motion;

    private void Start()
    {
        dragSpace = GetComponent<Image>();
        joyStickImagePos = joyStickImg.rectTransform.position;
    }
    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dragSpace.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            
            pos.x = (pos.x / dragSpace.rectTransform.sizeDelta.x);
            pos.y = (pos.y / dragSpace.rectTransform.sizeDelta.y);
            joyStickVector = new Vector3((pos.x*2)-1, 0, (pos.y*2)-1);
            joyStickVector = (joyStickVector.magnitude > 1) ? joyStickVector.normalized : joyStickVector;
            motion = joyStickVector.magnitude;
            motion = (motion < 0) ? -motion : motion;
            joyStickImg.rectTransform.anchoredPosition = new Vector3(joyStickVector.x * (dragSpace.rectTransform.sizeDelta.x / 2), joyStickVector.z * (dragSpace.rectTransform.sizeDelta.y / 2));
        }
    }
    public virtual void OnPointerUp(PointerEventData ped)
    {
        joyStickVector = Vector3.zero;
        joyStickImg.rectTransform.localPosition= new Vector3(50,50,0);
        motion = 0;
    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }
}
