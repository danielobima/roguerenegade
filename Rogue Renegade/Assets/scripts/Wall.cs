using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private Color color;
    private Color fadedColor;
    private float alpha = 1;
    public Material trans;
    private Material normal;

    private void Start()
    {
        color = trans.GetColor("_BaseColor");
        normal = gameObject.GetComponent<Renderer>().material;
        fadedColor = new Color(color.r, color.g, color.b, alpha);
    }
    public void ReduceOpacity()
    {
        
        if (alpha > 0.3f)
        {
            if (gameObject.GetComponent<Renderer>().material != trans)
            {
                gameObject.GetComponent<Renderer>().material = trans;
            }
            fadedColor = new Color(color.r, color.g, color.b, alpha);
            gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", fadedColor);
            alpha -= 5f * Time.deltaTime;
        }
        
    }
    
    public bool IncreaseOpacity()
    {
        if (alpha < 1f)
        {
            fadedColor = new Color(color.r, color.g, color.b, alpha);
            gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", fadedColor);
            alpha += 5f * Time.deltaTime;
            return false;
        }
        else
        {
            if(gameObject.GetComponent<Renderer>().material != normal)
            {
                gameObject.GetComponent<Renderer>().material = normal;
            }
            return true;
        }
    }
}
