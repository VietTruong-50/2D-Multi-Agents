using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScaleCheck : MonoBehaviour
{

    private float requireMass;
    public HashSet<GameObject> boxes;
    public bool isScaleCheck = false;

    void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.gameObject.CompareTag("Box"))
        {
            CheckTotalMass();
        }
    }

    public bool CheckTotalMass()
    {
        float sum = GetTotalMass();

        if (requireMass > sum)
        {
           return false;
        }

        return true;
    }

    public float GetTotalMass()
    {
        float sum = 0f;

        foreach (var item in boxes)
        {
            sum += item.GetComponent<Rigidbody2D>().mass;
        }
        return sum;
    }


    public void SetBoxes(HashSet<GameObject> boxes)
    {
        this.boxes = boxes;
    }

    public void SetRequireMass(float requireMass)
    {
        this.requireMass = requireMass;
    }

    public void SetScaleCheck(bool isScaleCheck)
    {
        this.isScaleCheck = isScaleCheck;
    }

    public bool GetScaleCheck()
    {
        return isScaleCheck;
    }
}
