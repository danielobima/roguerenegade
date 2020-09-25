using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClothSaveData
{
    public ClothItem[] clothItems;
    public float[][] colors;
    public bool lipstick;
    public bool sleeves;
    public bool chain;
    public bool male;

    public ClothSaveData(ClothItem[] clothItem, Color[] Colors,bool Lipstick,bool Sleeves,bool Chain,bool Male)
    {
        clothItems = clothItem;
        colors = new float[Colors.Length][];
        for (int i = 0; i < Colors.Length; i++)
        {
            colors[i] = new float[] { Colors[i].r, Colors[i].g, Colors[i].b, Colors[i].a };
        }
        lipstick = Lipstick;
        sleeves = Sleeves;
        chain = Chain;
        male = Male;
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
}
