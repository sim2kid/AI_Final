using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

public static class Knowledge
{
    public static List<CountInBox> boxes = new List<CountInBox>();
    public static CountInBox winnerNest() 
    {
        var b = boxes[0];
        foreach (CountInBox box in boxes) 
        {
            if (box.Count > b.Count) 
            {
                b = box;
            }
        }
        return b;
    }
}
