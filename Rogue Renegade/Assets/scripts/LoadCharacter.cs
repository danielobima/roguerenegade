using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LoadCharacter : NetworkBehaviour
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


    private PlayerMultiDetails playerMultiDetails;
    [SyncVar]
    public ClothSaveData clothData;
    private ClothSaveData SaveData;
    private GameMechMulti gameMechMulti;

    private void Start()
    {
        playerMultiDetails = GetComponent<PlayerMultiDetails>();
        if (playerMultiDetails.isMultiPlayer)
        {
            if (isLocalPlayer)
            {
                Load();
            }
            gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();
            GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject g in playerObjs)
            {
                if (!isLocalPlayer)
                {
                    LoadCharacter loadChara = g.GetComponent<LoadCharacter>();
                    loadChara.LoadExternal(loadChara.clothData);
                }
            }
           
        }
        else
            Load();
        
        
    }
    public void Load()
    {
        ClothSaveData clothSave = SaveSystem.loadClothes();
        

        if (clothSave != null)
        {

            SaveData = clothSave;
            if (playerMultiDetails.isMultiPlayer)
            {
                CmdLoad(SaveData, netId,gameObject);
            }

            SkinnedMeshRenderer[][] clothRendererArrays;
            

            male = clothSave.male;
            lipstick = clothSave.lipstick;
            sleeves = clothSave.sleeves;
            chain = clothSave.chain;

            setSkinColor(new Color(clothSave.body[0], clothSave.body[1], clothSave.body[2], clothSave.body[3]));
            if (lipstick)
            {
                randomLipstickColor(new Color(clothSave.lipstickColor[0], clothSave.lipstickColor[1], clothSave.lipstickColor[2], clothSave.lipstickColor[3]));
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
                        clothRendererArrays[i][clothSave.clothItems[i].clothNo].materials[z].SetColor("Color_3D7C8326", new Color(clothSave.clothItems[i].colors[z][0],
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
            meshRenderers[10].materials[0].SetColor("Color_3D7C8326", new Color(clothSave.eyes[0], clothSave.eyes[1], clothSave.eyes[2], clothSave.eyes[3]));//eyes
            meshRenderers[28].materials[0].SetColor("Color_3D7C8326", new Color(clothSave.trousers[0], clothSave.trousers[1], clothSave.trousers[2], clothSave.trousers[3]));//legs
            meshRenderers[2].materials[0].SetColor("Color_3D7C8326", new Color(clothSave.shoes[0], clothSave.shoes[1], clothSave.shoes[2], clothSave.shoes[3]));//shoes
        }
        else
        {
            //RandomizeCharacter();
            //Not found
        }

    }
    public void LoadExternal(ClothSaveData clothSave)
    {


        if (clothSave != null)
        {
            SkinnedMeshRenderer[][] clothRendererArrays;


            male = clothSave.male;
            lipstick = clothSave.lipstick;
            sleeves = clothSave.sleeves;
            chain = clothSave.chain;

            setSkinColor(new Color(clothSave.body[0], clothSave.body[1], clothSave.body[2], clothSave.body[3]));
            if (lipstick)
            {
                randomLipstickColor(new Color(clothSave.lipstickColor[0], clothSave.lipstickColor[1], clothSave.lipstickColor[2], clothSave.lipstickColor[3]));
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
                        clothRendererArrays[i][clothSave.clothItems[i].clothNo].materials[z].SetColor("Color_3D7C8326", new Color(clothSave.clothItems[i].colors[z][0],
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
            meshRenderers[10].materials[0].SetColor("Color_3D7C8326", new Color(clothSave.eyes[0], clothSave.eyes[1], clothSave.eyes[2], clothSave.eyes[3]));//eyes
            meshRenderers[28].materials[0].SetColor("Color_3D7C8326", new Color(clothSave.trousers[0], clothSave.trousers[1], clothSave.trousers[2], clothSave.trousers[3]));//legs
            meshRenderers[2].materials[0].SetColor("Color_3D7C8326", new Color(clothSave.shoes[0], clothSave.shoes[1], clothSave.shoes[2], clothSave.shoes[3]));//shoes
        }
        else
        {
            //RandomizeCharacter();
            //Not found
        }

    }

    [Command]
    private void CmdLoad(ClothSaveData clothSave, uint net,GameObject me)
    {
        RpcLoad(clothSave,  net);
        clothData = clothSave;
    }
    [ClientRpc]
    private void RpcLoad(ClothSaveData clothSave,uint net)
    {
        if(net == netId)
        {
            LoadExternal(clothSave);
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

        meshRenderers[3].material.SetColor("Color_3D7C8326", color); //female
        meshRenderers[29].material.SetColor("Color_3D7C8326", color); //male
        meshRenderers[9].material.SetColor("Color_3D7C8326", color); //hands
        meshRenderers[10].materials[3].SetColor("Color_3D7C8326", color); //kichwa
        bodyMat = meshRenderers[10].materials[3];


        if (!sleeves)
        {
            meshRenderers[0].material.SetColor("Color_3D7C8326", color);
            meshRenderers[1].material.SetColor("Color_3D7C8326", color);
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
        meshRenderers[10].materials[4].SetColor("Color_3D7C8326", color);
        //lipstickColor = colorPicker.color;
        //colorButtons["lipstick"].updateMyColor(lipstickColor);
    }
}
