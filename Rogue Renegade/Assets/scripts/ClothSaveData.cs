using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClothSaveData
{
    public ClothItem[] clothItems;
    //public float[][] colors;
    //body, upperArms, lowerArms, eyes, shoes, trousers, lipstickColor
    public float[] body;
    public float[] upperArms;
    public float[] lowerArms;
    public float[] eyes;
    public float[] shoes;
    public float[] trousers;
    public float[] lipstickColor;
    public bool lipstick;
    public bool sleeves;
    public bool chain;
    public bool male;

    public ClothSaveData(ClothItem[] clothItem, Color[] Colors,bool Lipstick,bool Sleeves,bool Chain,bool Male)
    {
        clothItems = clothItem;
        /*float[][] colors = new float[][] { body, upperArms, lowerArms, eyes, shoes, trousers, lipstickColor };
        for (int i = 0; i < Colors.Length; i++)
        {
            colors[i] = new float[] { Colors[i].r, Colors[i].g, Colors[i].b, Colors[i].a };
        }*/
        body = new float[] { Colors[0].r, Colors[0].g, Colors[0].b, Colors[0].a };
        upperArms = new float[] { Colors[1].r, Colors[1].g, Colors[1].b, Colors[1].a };
        lowerArms = new float[] { Colors[2].r, Colors[2].g, Colors[2].b, Colors[2].a };
        eyes = new float[] { Colors[3].r, Colors[3].g, Colors[3].b, Colors[3].a };
        shoes = new float[] { Colors[4].r, Colors[4].g, Colors[4].b, Colors[4].a };
        trousers = new float[] { Colors[5].r, Colors[5].g, Colors[5].b, Colors[5].a };
        lipstickColor = new float[] { Colors[6].r, Colors[6].g, Colors[6].b, Colors[6].a };

        lipstick = Lipstick;
        sleeves = Sleeves;
        chain = Chain;
        male = Male;
    }
    public ClothSaveData()
    {
        clothItems = new ClothItem[0];
        body = new float[0];
        upperArms = new float[0];
        lowerArms = new float[0];
        eyes = new float[0];
        shoes = new float[0];
        trousers = new float[0];
        lipstickColor = new float[0];
        lipstick = false;
        sleeves = true;
        chain = false;
        male = true;

    }
}

[System.Serializable]
public class ClothItem
{
    public string clothName;
    public int clothNo;
    public List<float[]> colors;
    public bool male;

    public ClothItem(string ClothName)
    {
        clothName = ClothName;
        clothNo = 0;
        colors = new List<float[]>();
        male = false;
    }
    public ClothItem()
    {
        clothName = "";
        clothNo = 0;
        colors = new List<float[]>();
        male = true;
    }
}
