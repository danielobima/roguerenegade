using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DevionGames.UIWidgets;
using TMPro;

public class CustomizeCharacter : MonoBehaviour
{
    public Color body;
    public Color upperArms;
    public Color lowerArms;
    public Color eyes;
    public Color shoes;
    public Color trousers;
    public Color lipstickColor;
    public bool lipstick;
    public bool sleeves;
    public bool chain;
    public bool male;

    public Material mainTorsoMat;
    public UIWidget colorPickerWidget;
    public string colorToChange = "";
    public FlexibleColorPicker colorPicker;
    public Image currentColorButton;
    public Image lipstickColorButton;

    public TextMeshProUGUI selectedHair;
    public TextMeshProUGUI selectedBody;
    public TextMeshProUGUI selectedTorso;
    public TextMeshProUGUI selectedEyebrow;
    public TextMeshProUGUI selectedGlasses;
    public TextMeshProUGUI selectedFacialHair;
    public Toggle lipstickToggle;
    public Toggle chainToggle;
    public Toggle sleevesToggle;

    public GameObject hat2Button;
    public List<Color> skinColors = new List<Color>();
    public int maleTorsoNumber;
    public int femaleTorsoNumber;

    public ClothItem glasses = new ClothItem("glasses");
    public ClothItem facialHair = new ClothItem("facialHair");
    public ClothItem hairHat = new ClothItem("hairHat");
    public ClothItem torso = new ClothItem("torso");
    public ClothItem eyebrow = new ClothItem("eyebrows");
   
    public Material lipstickMat;
    public Material bodyMat;

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

    public GameObject[] torsoColorButtonsMale;
    public GameObject[] torsoColorButtonsFemale;
    public Dictionary<string, ColorButton> colorButtons;

    public Color[] randomLipstickColors;
    public Color[] randomHairColors;
    private bool isBecauseOfRandomizing;

    private void Start()
    {
        colorPickerWidget.gameObject.SetActive(false);
        colorPickerWidget.Close();
        
        hairHat.colors.Add(new float[] {0,0,0,1 });
        LoadCharacter();
    }
    public void SaveCharacter()
    {
        if (male)
        {
            torso.colors = new List<float[]>();
            foreach (Material m in maleTorso[torso.clothNo].materials)
            {
                torso.colors.Add(new float[] { m.GetColor("_BaseColor").r, m.GetColor("_BaseColor").g, m.GetColor("_BaseColor").b, m.GetColor("_BaseColor").a });
            }
        }
        else
        {
            torso.colors = new List<float[]>();
            foreach (Material m in femaleTorso[torso.clothNo].materials)
            {
                torso.colors.Add(new float[] { m.GetColor("_BaseColor").r, m.GetColor("_BaseColor").g, m.GetColor("_BaseColor").b, m.GetColor("_BaseColor").a });
            }
        }
        SaveSystem.SaveClothes(new ClothSaveData(new ClothItem[] { glasses, facialHair, hairHat, torso, eyebrow },
            new Color[] { body, upperArms, lowerArms, eyes, shoes, trousers, lipstickColor }, lipstick, sleeves, chain, male));
    }
    public void LoadCharacter()
    {
        ClothSaveData clothSave = SaveSystem.loadClothes();
        
        if(clothSave != null)
        {
            SkinnedMeshRenderer[][] clothRendererArrays;
            ClothItem[] clothItems = { glasses, facialHair, hairHat, torso, eyebrow };
            male = clothSave.male;
            lipstick = clothSave.lipstick;
            sleeves = clothSave.sleeves;
            chain = clothSave.chain;
            glasses = clothSave.clothItems[0];
            facialHair = clothSave.clothItems[1];
            hairHat = clothSave.clothItems[2];
            torso = clothSave.clothItems[3];
            eyebrow = clothSave.clothItems[4];
            isBecauseOfRandomizing = true;
            lipstickToggle.isOn = lipstick;

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
                    if(s != null)
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
            meshRenderers[10].materials[3].SetColor("_BaseColor", new Color(clothSave.eyes[0], clothSave.eyes[1], clothSave.eyes[2], clothSave.eyes[3]));//eyes
            eyes = new Color(clothSave.eyes[0], clothSave.eyes[1], clothSave.eyes[2], clothSave.eyes[3]);
            meshRenderers[28].materials[0].SetColor("_BaseColor", new Color(clothSave.trousers[0], clothSave.trousers[1], clothSave.trousers[2], clothSave.trousers[3]));//legs
            trousers = new Color(clothSave.trousers[0], clothSave.trousers[1], clothSave.trousers[2], clothSave.trousers[3]);
            meshRenderers[2].materials[0].SetColor("_BaseColor", new Color(clothSave.shoes[0], clothSave.shoes[1], clothSave.shoes[2], clothSave.shoes[3]));//shoes
            shoes = new Color(clothSave.shoes[0], clothSave.shoes[1], clothSave.shoes[2], clothSave.shoes[3]);

            setUpTexts();
        }
        else
        {
            RandomizeCharacter();
        }
        

    }
    private void setUpTexts()
    {
        selectedHair.text = (hairHat.clothNo + 1).ToString();
        if (male)
        {
            selectedBody.text = "1";
        }
        else
        {
            selectedBody.text = "2";
        }
        selectedTorso.text = (torso.clothNo + 1).ToString();
        selectedEyebrow.text = (eyebrow.clothNo + 1).ToString();
        selectedGlasses.text = (glasses.clothNo + 1).ToString();
        selectedFacialHair.text = (facialHair.clothNo + 1).ToString();

    }
    
    public void RandomizeCharacter()
    {
        isBecauseOfRandomizing = true;
        body = skinColors[Random.Range(0, skinColors.Count)];
        setSkinColor(body);
       
        randomEyebrow(Random.Range(0, eyebrows.Length ));
        randomHairHat(Random.Range(0, hairHats.Length ));
        
        randomGlasses(Random.Range(0, sunglasses.Length));
        if(Random.Range(0, 2) == 0)
        {
            toggleBodyType(false);

            removeLipstick();
          
            lipstickToggle.isOn = false;
            randomTorso(Random.Range(0, maleTorso.Length));
            randomFacialHair(Random.Range(0, facialHairs.Length));
        }
        else
        {
            toggleBodyType(true);
            if (Random.Range(0, 2) == 0)
            {
                lipstick = false;
                removeLipstick();
                lipstickToggle.isOn = false;
            }
            else
            {
                lipstick = true;
                randomLipstickColor(randomLipstickColors[Random.Range(0, randomLipstickColors.Length)]);
                lipstickToggle.isOn = true;
            }
            randomFacialHair(0);
            randomTorso(Random.Range(0, femaleTorso.Length));
        }
        if(Random.Range(0, 2) == 0)
        {
            Sleeves(true);
            sleevesToggle.isOn = true;
        }
        else
        {
            Sleeves(false);
            sleevesToggle.isOn = false;
        }
        //random legs
        if(Random.Range(0, 2) == 0)
            trousers = RandomColor();
        else
            trousers = RandomColorDark();
        meshRenderers[28].materials[0].SetColor("_BaseColor", trousers);

        //random shoes
        if (Random.Range(0, 2) == 0)
            shoes = RandomColor();
        else
            shoes = RandomColorDark();
        meshRenderers[2].materials[0].SetColor("_BaseColor", shoes);

        if (Random.Range(0, 2) == 0)
        {
            toggleChain(false);
            chainToggle.isOn = false;
        }
        else{
            toggleChain(true);
            chainToggle.isOn = true;
        }
            

    }
    private Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
    private Color RandomColorDark()
    {
        return new Color(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
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
        body = color;
       
        meshRenderers[3].material.SetColor("_BaseColor", color); //female
        meshRenderers[29].material.SetColor("_BaseColor", color); //male
        meshRenderers[9].material.SetColor("_BaseColor", color); //hands
        meshRenderers[10].materials[0].SetColor("_BaseColor", color); //kichwa
        bodyMat = meshRenderers[10].materials[0];

        
        if (!sleeves)
        {
            meshRenderers[0].material.SetColor("_BaseColor", color);
            upperArms = color;
            meshRenderers[1].material.SetColor("_BaseColor", color);
            lowerArms = color;
        }
        if (!lipstick)
        {
            Material[] mats = meshRenderers[10].materials;
            mats[4] = mats[0];
            meshRenderers[10].materials = mats;
        }
        
    }
    public void Sleeves(bool sleeve)
    {
        sleeves = sleeve;
        if (sleeve)
        {
            
            meshRenderers[0].material = mainTorsoMat;
            meshRenderers[1].material = mainTorsoMat;
            upperArms = mainTorsoMat.GetColor("_BaseColor");
            lowerArms = mainTorsoMat.GetColor("_BaseColor");
            
             
        }
        else
        {
            meshRenderers[0].material = bodyMat;
            meshRenderers[1].material = bodyMat;
            upperArms = bodyMat.GetColor("_BaseColor");
            lowerArms = bodyMat.GetColor("_BaseColor");
        }
    }
    public void toggleSleeves(bool sleeve)
    {
        sleeves = sleeve;
        if (sleeve)
        {
            meshRenderers[0].material = mainTorsoMat;
            meshRenderers[1].material = mainTorsoMat;
            upperArms = mainTorsoMat.GetColor("_BaseColor");
            lowerArms = mainTorsoMat.GetColor("_BaseColor");
        }
        else
        {
            meshRenderers[0].material = bodyMat;
            meshRenderers[1].material = bodyMat;
            upperArms = bodyMat.GetColor("_BaseColor");
            lowerArms = bodyMat.GetColor("_BaseColor");
        }
    }
    public void closeColorPicker()
    {
        colorToChange = "";
        colorPickerWidget.Close();
        currentColorButton = null;
    }
    public void changeLipstickColor(bool on)
    {
        if (isBecauseOfRandomizing)
        {
            isBecauseOfRandomizing = false;
        }
        else
        {
            if (on)
            {
                lipstick = true;
                Material[] mats = meshRenderers[10].materials;
                mats[4] = lipstickMat;
                meshRenderers[10].materials = mats;
                meshRenderers[10].materials[4].SetColor("_BaseColor", lipstickColor);
                colorPickerWidget.Show();
                colorToChange = "lipstick";
                colorPicker.color = lipstickColor;
                //lipstickColor = colorPicker.color;
                currentColorButton = lipstickColorButton;
            }
            else
            {
                lipstick = false;
                Material[] mats = meshRenderers[10].materials;
                mats[4] = mats[0];
                meshRenderers[10].materials = mats;
                colorToChange = "";
                colorPickerWidget.Close();
            }
        }

    }
    private void removeLipstick()
    {
        lipstick = false;
        Material[] mats = meshRenderers[10].materials;
        mats[4] = mats[0];
        meshRenderers[10].materials = mats;
    }
    private void randomLipstickColor(Color color)
    {
        lipstickColor = color;
        Material[] mats = meshRenderers[10].materials;
        mats[4] = lipstickMat;
        meshRenderers[10].materials = mats;
        meshRenderers[10].materials[4].SetColor("_BaseColor", lipstickColor);
        //lipstickColor = colorPicker.color;
        //colorButtons["lipstick"].updateMyColor(lipstickColor);
    }
    private void nextClothItem(SkinnedMeshRenderer[] clothes, ClothItem item)
    {
        if(item.clothNo < clothes.Length - 1)
        {
            item.clothNo++;
            if (clothes[item.clothNo] != null)
            {
                clothes[item.clothNo].gameObject.SetActive(true);
                item.colors = new List<float[]>();
                foreach(Material m in clothes[item.clothNo].materials)
                {
                    item.colors.Add(new float[] { m.GetColor("_BaseColor").r, m.GetColor("_BaseColor").g, m.GetColor("_BaseColor").b, m.GetColor("_BaseColor").a });
                }
                
            }
            if (clothes[item.clothNo - 1] != null)
            {
                clothes[item.clothNo - 1].gameObject.SetActive(false);
            }
           
        }
        else
        {
            item.clothNo = 0;
            if (clothes[item.clothNo] != null)
            {
                clothes[item.clothNo].gameObject.SetActive(true);
                item.colors = new List<float[]>();
                foreach (Material m in clothes[item.clothNo].materials)
                {
                    item.colors.Add(new float[] { m.GetColor("_BaseColor").r, m.GetColor("_BaseColor").g, m.GetColor("_BaseColor").b, m.GetColor("_BaseColor").a });
                }
            }
            if (clothes[clothes.Length - 1] != null)
            {
                clothes[clothes.Length - 1].gameObject.SetActive(false);
            }
            
        }
    }
    private void randomClothItem(SkinnedMeshRenderer[] clothes, ClothItem item,int randomNo)
    {
        item.clothNo= randomNo;
        foreach(SkinnedMeshRenderer c in clothes)
        {
            if(c!= null)
            {
                c.gameObject.SetActive(false);
            }
        }

        try
        {
            if (clothes[item.clothNo] != null)
            {
                clothes[item.clothNo].gameObject.SetActive(true);
                if(item.clothName == "torso")
                {
                    foreach (Material m in clothes[item.clothNo].materials)
                    {
                        m.SetColor("_BaseColor", RandomColor());
                    }
                }
                if(item.clothName == "hairHat")
                {
                    if(item.clothNo != 7)
                    {
                        clothes[item.clothNo].material.SetColor("_BaseColor", randomHairColors[Random.Range(0,randomHairColors.Length)]);
                    }
                }
                item.colors = new List<float[]>();
                foreach (Material m in clothes[item.clothNo].materials)
                {
                    item.colors.Add(new float[] { m.GetColor("_BaseColor").r, m.GetColor("_BaseColor").g, m.GetColor("_BaseColor").b, m.GetColor("_BaseColor").a });
                }

            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.Log(item.clothName);
        }
       
    }
    private void prevClothItem(SkinnedMeshRenderer[] clothes, ClothItem item)
    {
        if (item.clothNo > 0)
        {
            item.clothNo--;
            if (clothes[item.clothNo] != null)
            {
                clothes[item.clothNo].gameObject.SetActive(true);
                item.colors = new List<float[]>();
                foreach (Material m in clothes[item.clothNo].materials)
                {
                    item.colors.Add(new float[] { m.GetColor("_BaseColor").r, m.GetColor("_BaseColor").g, m.GetColor("_BaseColor").b, m.GetColor("_BaseColor").a });
                }
            }
            if (clothes[item.clothNo + 1] != null)
            {
                clothes[item.clothNo + 1].gameObject.SetActive(false);
            }
            
        }
        else
        {
            item.clothNo = clothes.Length - 1;
            if (clothes[item.clothNo] != null)
            {
                clothes[item.clothNo].gameObject.SetActive(true);
                item.colors = new List<float[]>();
                foreach (Material m in clothes[item.clothNo].materials)
                {
                    item.colors.Add(new float[] { m.GetColor("_BaseColor").r, m.GetColor("_BaseColor").g, m.GetColor("_BaseColor").b, m.GetColor("_BaseColor").a });
                }
            }
            if (clothes[0] != null)
            {
                clothes[0].gameObject.SetActive(false);
            }
            
        }
    }
    public void nextBodyType()
    {
        if (male)
        {
            male = false;
            bodyTypes[1].gameObject.SetActive(true);
            bodyTypes[0].gameObject.SetActive(false);
            selectedBody.text = "2";
            torso.male = false;
            torso.clothNo = femaleTorsoNumber;
            selectedTorso.text = (femaleTorsoNumber + 1).ToString();
            foreach (GameObject g in torsoColorButtonsMale)
            {
                if (g != null)
                {
                    g.SetActive(false);
                }
            }
            if (torsoColorButtonsFemale[femaleTorsoNumber] != null)
            {
                torsoColorButtonsFemale[femaleTorsoNumber].SetActive(true);
            }
           
            mainTorsoMat = femaleTorso[torso.clothNo].material;
            Sleeves(sleeves);
            toggleChain(chain);
        }
        else
        {
            male = true;
            bodyTypes[0].gameObject.SetActive(true);
            bodyTypes[1].gameObject.SetActive(false);
            selectedBody.text = "1";
            torso.male = true;
            torso.clothNo = maleTorsoNumber;
            selectedTorso.text = (maleTorsoNumber + 1).ToString();
            foreach (GameObject g in torsoColorButtonsFemale)
            {
                if (g != null)
                {
                    g.SetActive(false);
                }
            }
            if (torsoColorButtonsMale[maleTorsoNumber] != null)
            {
                torsoColorButtonsMale[maleTorsoNumber].SetActive(true);
            }
            if (torso.clothNo != 4)
            {
                if (torso.clothNo != 1)
                {

                    mainTorsoMat = maleTorso[torso.clothNo].material;
                    Sleeves(sleeves);
                }
                else
                {


                    mainTorsoMat = maleTorso[torso.clothNo].materials[1];
                    Sleeves(sleeves);
                }
            }
            else
            {
                mainTorsoMat = bodyMat;
                Sleeves(sleeves);
            }
            toggleChain(chain);
        }
    }
    private void toggleBodyType(bool type)
    {
        male = type;
        if (male)
        {
            male = false;
            bodyTypes[1].gameObject.SetActive(true);
            bodyTypes[0].gameObject.SetActive(false);
            selectedBody.text = "2";
            torso.male = false;
            torso.clothNo = femaleTorsoNumber;
            selectedTorso.text = (femaleTorsoNumber + 1).ToString();
            foreach (GameObject g in torsoColorButtonsMale)
            {
                if (g != null)
                {
                    g.SetActive(false);
                }
            }
            if (torsoColorButtonsFemale[femaleTorsoNumber] != null)
            {
                torsoColorButtonsFemale[femaleTorsoNumber].SetActive(true);
            }
           
            toggleChain(chain);
            mainTorsoMat = femaleTorso[torso.clothNo].material;
            Sleeves(sleeves);
        }
        else
        {
            male = true;
            bodyTypes[0].gameObject.SetActive(true);
            bodyTypes[1].gameObject.SetActive(false);
            selectedBody.text = "1";
            torso.male = true;
            torso.clothNo = maleTorsoNumber;
            selectedTorso.text = (maleTorsoNumber + 1).ToString();
            foreach (GameObject g in torsoColorButtonsFemale)
            {
                if (g != null)
                {
                    g.SetActive(false);
                }
            }
            if (torsoColorButtonsMale[maleTorsoNumber] != null)
            {
                torsoColorButtonsMale[maleTorsoNumber].SetActive(true);
            }

            if (torso.clothNo != 4)
            {
                if (torso.clothNo != 1)
                {

                    mainTorsoMat = maleTorso[torso.clothNo].material;
                    Sleeves(sleeves);
                }
                else
                {


                    mainTorsoMat = maleTorso[torso.clothNo].materials[1];
                    Sleeves(sleeves);
                }
            }
            else
            {
                mainTorsoMat = bodyMat;
                Sleeves(sleeves);
            }
           
            toggleChain(chain);
        }
    }
    private void toggleBodyType2(bool type)
    {
        male = type;
        if (male)
        {
            male = false;
            bodyTypes[1].gameObject.SetActive(true);
            bodyTypes[0].gameObject.SetActive(false);
            selectedBody.text = "2";
            torso.male = false;
            torso.clothNo = femaleTorsoNumber;
            selectedTorso.text = (femaleTorsoNumber + 1).ToString();
            foreach (GameObject g in torsoColorButtonsMale)
            {
                if (g != null)
                {
                    g.SetActive(false);
                }
            }
            if (torsoColorButtonsFemale[femaleTorsoNumber] != null)
            {
                torsoColorButtonsFemale[femaleTorsoNumber].SetActive(true);
            }
            
           
        }
        else
        {
            male = true;
            bodyTypes[0].gameObject.SetActive(true);
            bodyTypes[1].gameObject.SetActive(false);
            selectedBody.text = "1";
            torso.male = true;
            torso.clothNo = maleTorsoNumber;
            selectedTorso.text = (maleTorsoNumber + 1).ToString();
            foreach (GameObject g in torsoColorButtonsFemale)
            {
                if (g != null)
                {
                    g.SetActive(false);
                }
            }
            if (torsoColorButtonsMale[maleTorsoNumber] != null)
            {
                torsoColorButtonsMale[maleTorsoNumber].SetActive(true);
            }

           
            
        }
    }

    public void nextTorso()
    {
        if (male)
        {
            nextClothItem(maleTorso, torso);
            torso.male = true;
            selectedTorso.text = (torso.clothNo + 1).ToString();
            maleTorsoNumber = torso.clothNo;
            if (torsoColorButtonsMale[torso.clothNo] != null)
            {
                torsoColorButtonsMale[torso.clothNo].SetActive(true);
            }
            if (torso.clothNo == 0)
            {
                if (torsoColorButtonsMale[torsoColorButtonsMale.Length - 1] != null)
                {
                    torsoColorButtonsMale[torsoColorButtonsMale.Length - 1].SetActive(false);
                }
            }
            else
            {
                if (torsoColorButtonsMale[torso.clothNo - 1] != null)
                {
                    torsoColorButtonsMale[torso.clothNo - 1].SetActive(false);
                }
            }
            if (torso.clothNo != 4)
            {
                if (torso.clothNo != 1)
                {

                    mainTorsoMat = maleTorso[torso.clothNo].material;
                    Sleeves(sleeves);
                }
                else
                {


                    mainTorsoMat = maleTorso[torso.clothNo].materials[1];
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
            nextClothItem(femaleTorso, torso);
            torso.male = false;
            selectedTorso.text = (torso.clothNo + 1).ToString();
            femaleTorsoNumber = torso.clothNo;
            if (torsoColorButtonsFemale[torso.clothNo] != null)
            {
                torsoColorButtonsFemale[torso.clothNo].SetActive(true);
            }
            if (torso.clothNo  == 0)
            {
                if (torsoColorButtonsFemale[torsoColorButtonsFemale.Length - 1] != null)
                {
                    torsoColorButtonsFemale[torsoColorButtonsFemale.Length - 1].SetActive(false);
                }
            }
            else
            {
                if (torsoColorButtonsFemale[torso.clothNo - 1] != null)
                {
                    torsoColorButtonsFemale[torso.clothNo - 1].SetActive(false);
                }
            }
            mainTorsoMat = femaleTorso[torso.clothNo].material;
            Sleeves(sleeves);
        }

    }
    public void prevTorso()
    {
        if (male)
        {
            prevClothItem(maleTorso, torso);
            torso.male = true;
            selectedTorso.text = (torso.clothNo + 1).ToString();
            maleTorsoNumber = torso.clothNo;
            if (torsoColorButtonsMale[torso.clothNo] != null)
            {
                torsoColorButtonsMale[torso.clothNo].SetActive(true);
            }
            if (torso.clothNo == torsoColorButtonsMale.Length - 1)
            {
                if (torsoColorButtonsMale[0] != null)
                {
                    torsoColorButtonsMale[0].SetActive(false);
                }
            }
            else
            {
                if (torsoColorButtonsMale[torso.clothNo + 1] != null)
                {
                    torsoColorButtonsMale[torso.clothNo + 1].SetActive(false);
                }
            }
            if (torso.clothNo != 4)
            {
                if (torso.clothNo != 1)
                {

                    mainTorsoMat = maleTorso[torso.clothNo].material;
                    Sleeves(sleeves);
                }
                else
                {


                    mainTorsoMat = maleTorso[torso.clothNo].materials[1];
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
            prevClothItem(femaleTorso, torso);
            torso.male = false;
            selectedTorso.text = (torso.clothNo + 1).ToString();
            femaleTorsoNumber = torso.clothNo;
            if (torsoColorButtonsFemale[torso.clothNo] != null)
            {
                torsoColorButtonsFemale[torso.clothNo].SetActive(true);
            }
            if (torso.clothNo  == torsoColorButtonsFemale.Length - 1)
            {
                if (torsoColorButtonsFemale[0] != null)
                {
                    torsoColorButtonsFemale[0].SetActive(false);
                }
            }
            else
            {
                if (torsoColorButtonsFemale[torso.clothNo + 1] != null)
                {
                    torsoColorButtonsFemale[torso.clothNo + 1].SetActive(false);
                }
            }
            mainTorsoMat = femaleTorso[torso.clothNo].material;
            Sleeves(sleeves);
        }
    }
    public void randomTorso(int Number)
    {
        foreach(GameObject g in torsoColorButtonsFemale)
        {
            if(g!= null)
            {
                g.SetActive(false);
            }
        }
        foreach (GameObject g in torsoColorButtonsMale)
        {
            if (g != null)
            {
                g.SetActive(false);
            }
        }
        if (male)
        {
            randomClothItem(maleTorso, torso, Number);
            torso.male = true;
            selectedTorso.text = (torso.clothNo + 1).ToString();
            maleTorsoNumber = torso.clothNo;
            if (torsoColorButtonsMale[torso.clothNo] != null)
            {
                torsoColorButtonsMale[torso.clothNo].SetActive(true);
            }
            if (torso.clothNo != 4)
            {
                if (torso.clothNo != 1)
                {

                    mainTorsoMat = maleTorso[torso.clothNo].material;
                    Sleeves(sleeves);
                }
                else
                {


                    mainTorsoMat = maleTorso[torso.clothNo].materials[1];
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
            randomClothItem(femaleTorso, torso, Number);
            torso.male = false;
            selectedTorso.text = (torso.clothNo + 1).ToString();
            femaleTorsoNumber = torso.clothNo;
            if (torsoColorButtonsFemale[torso.clothNo] != null)
            {
                torsoColorButtonsFemale[torso.clothNo].SetActive(true);
            }
           
            mainTorsoMat = femaleTorso[torso.clothNo].material;
            Sleeves(sleeves);
        }

    }
    public void randomHairHat(int Number)
    {
        randomClothItem(hairHats, hairHat, Number);
        selectedHair.text = (hairHat.clothNo + 1).ToString();
        if (hairHat.clothNo != 7)
        {
            hat2Button.SetActive(false);
            
        }
        else
        {
            hat2Button.SetActive(true);
        }
    }
    public void nextHairHat()
    {
        nextClothItem(hairHats, hairHat);
        selectedHair.text = (hairHat.clothNo + 1).ToString();
        if(hairHat.clothNo != 7)
        {
            hat2Button.SetActive(false);
        }
        else
        {
            hat2Button.SetActive(true);
        }
    }
    
    public void prevHairHat()
    {
        prevClothItem(hairHats, hairHat);
        selectedHair.text = (hairHat.clothNo + 1).ToString();
        if (hairHat.clothNo != 7)
        {
            hat2Button.SetActive(false);
        }
        else
        {
            hat2Button.SetActive(true);
        }
    }
    public void randomFacialHair(int Number)
    {
        randomClothItem(facialHairs, facialHair, Number);
        selectedFacialHair.text = (facialHair.clothNo + 1).ToString();
        
    }
    public void nextFacialHair()
    {
        nextClothItem(facialHairs, facialHair);
        selectedFacialHair.text = (facialHair.clothNo + 1).ToString();
        
    }

    public void prevFacialHair()
    {
        prevClothItem(facialHairs, facialHair);
        selectedFacialHair.text = (facialHair.clothNo + 1).ToString();
        
       
    }
    public void randomEyebrow(int Number)
    {
        randomClothItem(eyebrows, eyebrow, Number);
        selectedEyebrow.text = (eyebrow.clothNo + 1).ToString();
        
    }
    public void nextEyebrow()
    {
        nextClothItem(eyebrows, eyebrow);
        selectedEyebrow.text = (eyebrow.clothNo + 1).ToString();
       
    }

    public void prevEyebrow()
    {
        prevClothItem(eyebrows, eyebrow);
        selectedEyebrow.text = (eyebrow.clothNo + 1).ToString();
        
    }
    public void randomGlasses(int Number)
    {
        randomClothItem(sunglasses, glasses, Number);
        selectedGlasses.text = (glasses.clothNo + 1).ToString();
    }
    public void nextGlasses()
    {
        nextClothItem(sunglasses, glasses);
        selectedGlasses.text = (glasses.clothNo + 1).ToString();

    }

    public void prevGlasses()
    {
        prevClothItem(sunglasses, glasses);
        selectedGlasses.text = (glasses.clothNo + 1).ToString();

    }



    private void Update()
    {
        switch (colorToChange)
        {
            case "lipstick":
                if (lipstick)
                {
                    lipstickColor = colorPicker.color;
                    meshRenderers[10].materials[4].SetColor("_BaseColor", colorPicker.color);
                    currentColorButton.color = colorPicker.color;
                }
                break;
            case "eyes":
                eyes = colorPicker.color;
                meshRenderers[10].materials[3].SetColor("_BaseColor", colorPicker.color);
                currentColorButton.color = colorPicker.color;
                break;
            case "skin":
                setSkinColor(colorPicker.color);
                currentColorButton.color = colorPicker.color;
                break;
            case "hairHat1":
                hairHat.colors[0] = new float[] { colorPicker.color.r, colorPicker.color.g , colorPicker.color.b , colorPicker.color.a };
                
                hairHats[hairHat.clothNo].materials[0].SetColor("_BaseColor", colorPicker.color);
                currentColorButton.color = colorPicker.color;
                break;
            case "hat2":
                hairHat.colors[1] = new float[] { colorPicker.color.r, colorPicker.color.g, colorPicker.color.b, colorPicker.color.a };
                hairHats[hairHat.clothNo].materials[1].SetColor("_BaseColor", colorPicker.color);
                currentColorButton.color = colorPicker.color;
                break;
            case "tshirt female":
                femaleTorso[2].material.SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                mainTorsoMat = femaleTorso[2].material;
                Sleeves(sleeves);
                break;
            case "tshirt male":
                maleTorso[2].material.SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                mainTorsoMat = maleTorso[2].material;
                Sleeves(sleeves);
                break;
            case "shirt female":
                femaleTorso[1].materials[0].SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                mainTorsoMat = femaleTorso[1].materials[0];
                Sleeves(sleeves);
                break;
            case "shirt male":
                maleTorso[1].materials[1].SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                mainTorsoMat = maleTorso[1].materials[1];
                Sleeves(sleeves);
                break;
            case "shirt buttons female":
                femaleTorso[1].materials[1].SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                break;
            case "shirt buttons male":
                maleTorso[1].materials[0].SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                break;
            case "coat female":
                femaleTorso[0].materials[0].SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                mainTorsoMat = femaleTorso[0].materials[0];
                Sleeves(sleeves);
                break;
            case "coat buttons female":
                femaleTorso[0].materials[3].SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                break;
            case "coat belt":
                femaleTorso[0].materials[2].SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                break;
            case "coat male":
                maleTorso[0].materials[0].SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                mainTorsoMat = maleTorso[0].materials[0];
                Sleeves(sleeves);
                break;
            case "coat buttons male":
                maleTorso[0].materials[3].SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                break;
            case "vest female":
                femaleTorso[3].material.SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                mainTorsoMat = femaleTorso[3].material;
                Sleeves(sleeves);
                break;
            case "vest male":
                maleTorso[3].material.SetColor("_BaseColor", colorPicker.color);
                //we'll set the colors in the cloth item when we are saving the character.
                currentColorButton.color = colorPicker.color;
                mainTorsoMat = maleTorso[3].material;
                Sleeves(sleeves);
                break;
            case "legs":
                trousers = colorPicker.color;
                meshRenderers[28].materials[0].SetColor("_BaseColor", colorPicker.color);
                currentColorButton.color = colorPicker.color;
                break;
            case "shoes":
                shoes = colorPicker.color;
                meshRenderers[2].materials[0].SetColor("_BaseColor", colorPicker.color);
                currentColorButton.color = colorPicker.color;
                break;
            case "facial hair":
                facialHair.colors[0] = new float[] { colorPicker.color.r, colorPicker.color.g, colorPicker.color.b, colorPicker.color.a };
                facialHairs[facialHair.clothNo].materials[0].SetColor("_BaseColor", colorPicker.color);
                currentColorButton.color = colorPicker.color;
                break;
            case "eyebrows":
                eyebrow.colors[0] = new float[] { colorPicker.color.r, colorPicker.color.g, colorPicker.color.b, colorPicker.color.a };
                eyebrows[eyebrow.clothNo].materials[0].SetColor("_BaseColor", colorPicker.color);
                currentColorButton.color = colorPicker.color;
                break;
            case "glassesGlass":
                glasses.colors[0] = new float[] { colorPicker.color.r, colorPicker.color.g, colorPicker.color.b, 0.7f };
                sunglasses[glasses.clothNo].materials[0].SetColor("_BaseColor", new Color(colorPicker.color.r, colorPicker.color.g, colorPicker.color.b, 0.7f));
                currentColorButton.color = colorPicker.color;
                break;
            case "glassesRim":
                glasses.colors[1] = new float[] { colorPicker.color.r, colorPicker.color.g, colorPicker.color.b, 0.7f };
                sunglasses[glasses.clothNo].materials[1].SetColor("_BaseColor", colorPicker.color);
                currentColorButton.color = colorPicker.color;
                break;


        }
    }
    
}
