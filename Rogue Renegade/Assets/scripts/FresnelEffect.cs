using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FresnelEffect : MonoBehaviour
{


    [Header("For Objects with only one meshrenderer use fresnel highlight")]
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private MeshRenderer[] meshRenderers;
    private List<Material> fresnels;
    private Material[][] defaultMats;
    private Color[] defaultColors;
    public Color fresnelColor;
    public Shader fresnelShader;
    public float speed;
    private bool isFresnating;
    public Vector2 minMax = new Vector2(0, 1);

   
    public bool notSkinned = false;
    public bool onObject = false;
    private SkinnedMeshRenderer objectSkinnedRenderer;
    private Renderer objectRenderer;
    private Material[] defaultObjMats;

    private void Start()
    {

    }

    /// <summary>
    /// Use this if you will switch off manually
    /// </summary>
    public void fresnate()
    {
        if (!isFresnating)
        {

            if (!notSkinned)
            {
                skinnedMeshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
                /*defaultMats = new Material[skinnedMeshRenderers.Length][];
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    defaultMats[i] = skinnedMeshRenderers[i].materials;
                    fresnels = new List<Material>(skinnedMeshRenderers[i].materials);
                    Material frenselMat = new Material(fresnelShader);
                    frenselMat.SetFloat("Vector1_D75BD163", speed);
                    frenselMat.SetColor("Color_3539850B", fresnelColor);
                    frenselMat.SetVector("Vector2_B0A441B", minMax);
                    fresnels.Add(frenselMat);
                    skinnedMeshRenderers[i].materials = fresnels.ToArray();
                    Debug.Log(skinnedMeshRenderers[i].gameObject.name + " : " +
                        skinnedMeshRenderers[i].materials[skinnedMeshRenderers[i].materials.Length - 1].name);
                }*/

                foreach(SkinnedMeshRenderer meshRenderer in skinnedMeshRenderers)
                {
                    foreach (Material m in meshRenderer.materials)
                    {
                        m.SetVector("Vector2_B0A441B", minMax);
                        m.SetFloat("Vector1_D75BD163", speed);
                        m.SetColor("Color_3539850B", fresnelColor);
                    }
                }
            }
            else
            {
                meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
                /*defaultMats = new Material[meshRenderers.Length][];
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    defaultMats[i] = meshRenderers[i].materials;
                    fresnels = new List<Material>(meshRenderers[i].materials);
                    Material frenselMat = new Material(fresnelShader);
                    frenselMat.SetFloat("Vector1_D75BD163", speed);
                    frenselMat.SetColor("Color_3539850B", fresnelColor);
                    frenselMat.SetVector("Vector2_B0A441B", minMax);
                    fresnels.Add(frenselMat);
                    meshRenderers[i].materials = fresnels.ToArray();
                    Debug.Log(meshRenderers[i].gameObject.name + " : " +
                        meshRenderers[i].materials[meshRenderers[i].materials.Length - 1].name);
                }*/

                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    foreach (Material m in meshRenderer.materials)
                    {
                        m.SetVector("Vector2_B0A441B", minMax);
                        m.SetFloat("Vector1_D75BD163", speed);
                        m.SetColor("Color_3539850B", fresnelColor);
                    }
                }
            }
            if (onObject)
            {
                if (!notSkinned)
                {
                    objectRenderer = GetComponent<SkinnedMeshRenderer>();
                }
                else
                {
                    objectRenderer = GetComponent<MeshRenderer>();
                }
                /*defaultObjMats = objectRenderer.materials;
                fresnels = new List<Material>(objectRenderer.materials);
                Material frenselMat = new Material(fresnelShader);
                frenselMat.SetFloat("Vector1_D75BD163", speed);
                frenselMat.SetColor("Color_3539850B", fresnelColor);
                frenselMat.SetVector("Vector2_B0A441B", minMax);
                fresnels.Add(frenselMat);
                objectRenderer.materials = fresnels.ToArray();*/

                foreach (Material m in objectRenderer.materials)
                {
                    m.SetVector("Vector2_B0A441B", minMax);
                    m.SetFloat("Vector1_D75BD163", speed);
                    m.SetColor("Color_3539850B", fresnelColor);
                }
            }

            
            isFresnating = true;
        }

    }
  
    /// <summary>
    /// use this if you will switch off automatically
    /// </summary>
    /// <param name="duration">How long till it switches off</param>
    public void fresnate(float duration)
    {

        fresnate();
        Invoke("defresnate", duration);
    }


    public void defresnate()
    {
        if (isFresnating)
        {
            if (!notSkinned)
            {
                foreach (SkinnedMeshRenderer meshRenderer in skinnedMeshRenderers)
                {
                    foreach (Material m in meshRenderer.materials)
                    {
                        m.SetVector("Vector2_B0A441B", new Vector4());
                    }
                }
            }
            else
            {
                /*for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].materials = defaultMats[i];

                }*/
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    foreach (Material m in meshRenderer.materials)
                    {
                        m.SetVector("Vector2_B0A441B", new Vector4());
                    }
                }
            }
            if (onObject)
            {
                foreach (Material m in objectRenderer.materials)
                {
                    m.SetVector("Vector2_B0A441B", new Vector4());
                }
            }

            isFresnating = false;
        }
    }
}
