using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int Owner = -1;

    public Material pA;
    public Material pB;
    public Material pC;
    public Material pD;


    public void SetOwner(int player) 
    {
        Owner = player;
        var ren = GetComponent<Renderer>();
        if(Owner == 0)
            ren.material = pA;
        if (Owner == 1)
            ren.material = pB;
        if (Owner == 2)
            ren.material = pC;
        if (Owner == 3)
            ren.material = pD;
    }
}
