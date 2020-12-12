using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FresnelHighlight : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material[] defaultMats;
    private Color defaultColor;
    public Color fresnelColor;
    public Shader fresnelShader;
    public float speed;
    public bool isFresnating = false;
    private List<Material> fresnels;
    

    /// <summary>
    /// use this if you will switch off manually
    /// </summary>
    public void fresnate()
    {
        if (!isFresnating)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            

            foreach(Material m in meshRenderer.materials)
            {
                m.SetVector("Vector2_B0A441B", new Vector4(0, 1, 0, 0));
                m.SetFloat("Vector1_D75BD163", speed);
                m.SetColor("Color_3539850B", fresnelColor);
            }
            isFresnating = true;
        }
    }
    /// <summary>
    /// use this if you want to switch off automatically
    /// </summary>
    /// <param name="duration">How long till it switches off</param>
    public void fresnate(float duration)
    {
        fresnate();
        Invoke("defresnate", duration);
    }

    public void defresnate()
    {
        foreach (Material m in meshRenderer.materials)
        {
            m.SetVector("Vector2_B0A441B", new Vector4());
            
        }
        isFresnating = false;
    }
}
