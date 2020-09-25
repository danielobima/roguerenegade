using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public CustomizeCharacter customizeCharacter;
    public string ColorName;
    private Image image;

    private void Start()
    {
        //customizeCharacter.colorButtons.Add(ColorName, GetComponent<ColorButton>());
        image = GetComponent<Image>();
    }
    public void changeColor(string ColorName)
    {

        if (!customizeCharacter.colorPickerWidget.IsVisible)
        {
            customizeCharacter.colorPickerWidget.Show();
            customizeCharacter.colorToChange = ColorName;
            customizeCharacter.currentColorButton = image;
        }
        else
        {
            customizeCharacter.colorPickerWidget.Close();
            customizeCharacter.colorToChange = "";
            customizeCharacter.currentColorButton = null;
        }
        
    }
    public void updateMyColor(Color newColor)
    {
        image.color = newColor;
    }
    
}
