using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : MonoBehaviour
{
    public Material NormalMat;
    public Material swayMat;
    public bool priority;
    private bool changedUp = false;
    private bool changedDown = false;
    private MeshRenderer Mrenderer;
    void Start()
    {
        Mrenderer = GetComponent<MeshRenderer>();
#if ( UNITY_IPHONE || UNITY_ANDROID )
        if (!priority)
        {
            Destroy(gameObject);
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if(QualitySettings.GetQualityLevel() > 3)
        {
            if (!changedUp)
            {
                Mrenderer.material = swayMat;
                changedUp = true;
                changedDown = false;
            }
        }
        else
        {
            if (!changedDown)
            {
                Mrenderer.material = NormalMat;
                changedDown = true;
                changedUp = false;
            }
        }
    }
}
