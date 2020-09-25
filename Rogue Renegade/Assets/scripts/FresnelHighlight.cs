using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FresnelHighlight : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material fresnel;
    private Material defaultMat;
    private Color defaultColor;
    public Color fresnelColor;
    public Shader fresnelShader;
    public float speed;
    public bool isFresnating = false;

    public void fresnate()
    {
        if (!isFresnating)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            defaultMat = meshRenderer.material;
            defaultColor = meshRenderer.material.color;
            fresnel = new Material(fresnelShader);
            fresnel.SetFloat("Vector1_D75BD163", speed);
            fresnel.SetColor("Color_3D7C8326", defaultColor);
            fresnel.SetColor("Color_3539850B", fresnelColor);
            meshRenderer.material = fresnel;
            isFresnating = true;
        }
    }
    public void defresnate()
    {
        meshRenderer.material = defaultMat;
        isFresnating = false;
    }
}
