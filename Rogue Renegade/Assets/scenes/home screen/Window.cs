using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public Animator animator;
    
    
    public void showWindow(string trigger)
    {
        gameObject.SetActive(true);
        animator.SetTrigger(trigger);
    }
    public void hideWindow(string trigger)
    {
        animator.SetTrigger(trigger);
    }
    public void hide()
    {
        gameObject.SetActive(false);
    }
}
public static class WindowAnimationTriggers
{
    public static readonly string popIn = "popIn";
    public static readonly string popOut = "popOut";
}

