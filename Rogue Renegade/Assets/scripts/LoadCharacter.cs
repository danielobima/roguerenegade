using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    

    public SkinnedMeshRenderer[] meshRenderers;
    public SkinnedMeshRenderer[] hairHats;
    public SkinnedMeshRenderer[] facialHairs;
    public SkinnedMeshRenderer[] eyebrows;
    public SkinnedMeshRenderer[] bodyTypes;
    public SkinnedMeshRenderer[] femaleTorso;
    public SkinnedMeshRenderer[] maleTorso;
    public SkinnedMeshRenderer[] sunglasses;
    public SkinnedMeshRenderer chainFemale;
    public SkinnedMeshRenderer chainMale;

    public Material lipstickMat;
    public Material bodyMat;
    private Material mainTorsoMat;

    private bool lipstick;
    private bool sleeves;
    private bool chain;
    private bool male;

    private void Start()
    {
        Load();
    }
    public void Load()
    {
        ClothSaveData clothSave = SaveSystem.loadClothes();

        if (clothSave != null)
        {
            SkinnedMeshRenderer[][] clothRendererArrays;
            
            male = clothSave.male;
            lipstick = clothSave.lipstick;
            sleeves = clothSave.sleeves;
            chain = clothSave.chain;

            setSkinColor(new Color(clothSave.colors[0][0], clothSave.colors[0][1], clothSave.colors[0][2], clothSave.colors[0][3]));
            if (lipstick)
            {
                randomLipstickColor(new Color(clothSave.colors[6][0], clothSave.colors[6][1], clothSave.colors[6][2], clothSave.colors[6][3]));
            }
            else
            {
                removeLipstick();
            }
            toggleChain(chain);

            if (clothSave.male)
            {
                clothRendererArrays = new SkinnedMeshRenderer[][] { sunglasses, facialHairs, hairHats, maleTorso, eyebrows };
            }
            else
            {
                clothRendererArrays = new SkinnedMeshRenderer[][] { sunglasses, facialHairs, hairHats, femaleTorso, eyebrows };
            }

            for (int i = 0; i < clothRendererArrays.Length; i++)
            {
                foreach (SkinnedMeshRenderer s in clothRendererArrays[i])
                {
                    if (s != null)
                    {
                        s.gameObject.SetActive(false);
                    }
                }
                if (clothRendererArrays[i][clothSave.clothItems[i].clothNo] != null)
                {
                    clothRendererArrays[i][clothSave.clothItems[i].clothNo].gameObject.SetActive(true);
                    for (int z = 0; z < clothRendererArrays[i][clothSave.clothItems[i].clothNo].materials.Length; z++)
                    {
                        clothRendererArrays[i][clothSave.clothItems[i].clothNo].materials[z].SetColor("_BaseColor", new Color(clothSave.clothItems[i].colors[z][0],
                            clothSave.clothItems[i].colors[z][1],
                            clothSave.clothItems[i].colors[z][2],
                            clothSave.clothItems[i].colors[z][3]));
                    }
                }






                if (i == 3)
                {
                    if (male)
                    {
                        if (clothSave.clothItems[i].clothNo != 4)
                        {
                            if (clothSave.clothItems[i].clothNo != 1)
                            {

                                mainTorsoMat = clothRendererArrays[i][clothSave.clothItems[i].clothNo].materials[0];
                                Sleeves(sleeves);
                            }
                            else
                            {


                                mainTorsoMat = clothRendererArrays[i][clothSave.clothItems[i].clothNo].materials[1];
                                Sleeves(sleeves);
                            }
                        }
                        else
                        {
                            mainTorsoMat = bodyMat;
                            Sleeves(sleeves);
                        }
                    }
                    else
                    {
                        mainTorsoMat = clothRendererArrays[i][clothSave.clothItems[i].clothNo].materials[0];
                        Sleeves(sleeves);
                    }
                }


            }
            toggleBodyType2(!male);
            meshRenderers[10].materials[0].SetColor("_BaseColor", new Color(clothSave.colors[3][0], clothSave.colors[3][1], clothSave.colors[3][2], clothSave.colors[3][3]));//eyes
            meshRenderers[28].materials[0].SetColor("_BaseColor", new Color(clothSave.colors[5][0], clothSave.colors[5][1], clothSave.colors[5][2], clothSave.colors[5][3]));//legs
            meshRenderers[2].materials[0].SetColor("_BaseColor", new Color(clothSave.colors[4][0], clothSave.colors[4][1], clothSave.colors[4][2], clothSave.colors[4][3]));//shoes
        }
        else
        {
            //RandomizeCharacter();
            //Not found
        }

    }
    private void removeLipstick()
    {
        lipstick = false;
        Material[] mats = meshRenderers[10].materials;
        mats[4] = mats[3];
        meshRenderers[10].materials = mats;
    }

    private void toggleBodyType2(bool type)
    {
        male = type;
        if (male)
        {
            male = false;
            bodyTypes[1].gameObject.SetActive(true);
            bodyTypes[0].gameObject.SetActive(false);
           


        }
        else
        {
            male = true;
            bodyTypes[0].gameObject.SetActive(true);
            bodyTypes[1].gameObject.SetActive(false);
            



        }
    }

    public void Sleeves(bool sleeve)
    {
        sleeves = sleeve;
        if (sleeve)
        {

            meshRenderers[0].material = mainTorsoMat;
            meshRenderers[1].material = mainTorsoMat;


        }
        else
        {
            meshRenderers[0].material = bodyMat;
            meshRenderers[1].material = bodyMat;
        }
    }

    public void toggleChain(bool chailOn)
    {
        chain = chailOn;
        if (chailOn)
        {
            if (male)
            {
                chainMale.gameObject.SetActive(true);
            }
            else
            {
                chainFemale.gameObject.SetActive(true);
            }
        }
        else
        {
            if (male)
            {
                chainMale.gameObject.SetActive(false);
            }
            else
            {
                chainFemale.gameObject.SetActive(false);
            }
        }
    }

    public void setSkinColor(Color color)
    {

        meshRenderers[3].material.SetColor("_BaseColor", color); //female
        meshRenderers[29].material.SetColor("_BaseColor", color); //male
        meshRenderers[9].material.SetColor("_BaseColor", color); //hands
        meshRenderers[10].materials[3].SetColor("_BaseColor", color); //kichwa
        bodyMat = meshRenderers[10].materials[0];


        if (!sleeves)
        {
            meshRenderers[0].material.SetColor("_BaseColor", color);
            meshRenderers[1].material.SetColor("_BaseColor", color);
        }
        if (!lipstick)
        {
            Material[] mats = meshRenderers[10].materials;
            mats[4] = mats[3];
            meshRenderers[10].materials = mats;
        }

    }

    private void randomLipstickColor(Color color)
    {
        Material[] mats = meshRenderers[10].materials;
        mats[4] = lipstickMat;
        meshRenderers[10].materials = mats;
        meshRenderers[10].materials[4].SetColor("_BaseColor", color);
        //lipstickColor = colorPicker.color;
        //colorButtons["lipstick"].updateMyColor(lipstickColor);
    }
}
