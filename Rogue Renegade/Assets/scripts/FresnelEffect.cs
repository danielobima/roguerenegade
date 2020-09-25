using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FresnelEffect : MonoBehaviour
{
    private SkinnedMeshRenderer[] meshRenderers;
    private Material[] fresnels;
    private Material[] defaultMats;
    private Color[] defaultColors;
    public Color fresnelColor;
    public Shader fresnelShader;
    public float duration;
    public float speed;
    private bool isFresnating;
    public Vector2 minMax = new Vector2(0, 1);

    public void fresnate()
    {
        if (!isFresnating)
        {
            meshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            fresnels = new Material[meshRenderers.Length];
            defaultMats = new Material[meshRenderers.Length];
            defaultColors = new Color[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                defaultMats[i] = meshRenderers[i].material;
                defaultColors[i] = meshRenderers[i].material.color;
                fresnels[i] = new Material(fresnelShader);
                fresnels[i].SetFloat("Vector1_D75BD163", speed);
                fresnels[i].SetColor("Color_3D7C8326", defaultColors[i]);
                fresnels[i].SetColor("Color_3539850B", fresnelColor);
                fresnels[i].SetVector("Vector2_B0A441B", minMax);
                meshRenderers[i].material = fresnels[i];

            }
            Invoke("defresnate", duration);
            isFresnating = true;
        }
    }
    private void defresnate()
    {
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material  = defaultMats[i];
        }
        isFresnating = false;
    }
}
