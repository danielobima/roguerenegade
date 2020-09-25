using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SkinColors : MonoBehaviour
{
    public CustomizeCharacter customizeCharacter;
    private Toggle toggle;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        
        toggle.onValueChanged.AddListener((value)=>{ SetColor(value); });
        customizeCharacter.skinColors.Add(toggle.colors.normalColor);
    }
    
   
    public void SetColor(bool selected)
    {
        if (selected)
        {
            
            customizeCharacter.setSkinColor(toggle.colors.normalColor);
        }
    }
    

}
