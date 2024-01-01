using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmEntity
{
    public int index;
    public float minPosition;
    public float maxPosition;


    public ArmEntity(int index, float maxPosition, float minPosition)
    {
        this.index = index;
        this.minPosition = minPosition;
        this.maxPosition = maxPosition;
    }

}
