using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertSign : MonoBehaviour
{
    public bool isShowing;
    public bool isAdding;
    public bool isSubtracting;
    private GameObject theMeOnCanvas;
    public SVGImage img;
    public Animator animator;
    
    private void FixedUpdate()
    {
        if (isAdding)
        {
            becomeRed();
        }
        if (isSubtracting)
        {
            becomeWhite();
        }
    }
    private void becomeRed()
    {
        img.CrossFadeColor(Color.red, 3, false, false);
        float bg = img.color.b;
        if (bg <= 0)
        {
            isAdding = false;
        }
    }
    public void pulse()
    {
        animator.SetTrigger("alert");
        

    }
    private void becomeWhite()
    {
        img.CrossFadeColor(Color.white, 3, false, false);
        float bg = img.color.b;
        if (bg >= 1)
        {
            isSubtracting = false;
            StartCoroutine(turnOffAlertSign(1));
        }
    }
    public void stopPulse()
    {

        animator.SetTrigger("alert off");
    }
    IEnumerator turnOffAlertSign(int wait = 7)
    {
        yield return new WaitForSeconds(wait);
        gameObject.SetActive(false);
        isShowing = false;
        animator.SetTrigger("alert off");
        img.color = new Color(1, 1, 1);
    }
}

