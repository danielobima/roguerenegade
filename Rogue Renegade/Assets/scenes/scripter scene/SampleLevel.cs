using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleLevel : MonoBehaviour
{

    public Barrier[] barriers;

   
    void FixedUpdate()
    {
        foreach(Barrier b in barriers)
        {
            if (b.playerCrossed())
            {
                if(b.BarrierNo < 5)
                {
                    string[] triggers = { "Enemy on" };
                    b.triggerBarrier(triggers);
                }
                else
                {

                }
            }
        }
    }
}
