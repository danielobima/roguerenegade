using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimHelper : MonoBehaviour
{

    private Target t;
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponentInParent<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.touchCount >= 1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {

                if (Input.touches[i].phase == TouchPhase.Began)
                {
                    Touch touch = Input.GetTouch(i);
                    RaycastHit hit = new RaycastHit();
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 9) | (1 << 15)))
                    {
                        if (hit.collider.gameObject == this.gameObject)
                        {
                            
                            t.enemyTouched();

                        }

                    }
                }
            }



        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 9) | (1 << 15)))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    t.enemyTouched();

                }

            }
        }*/
    }

}
