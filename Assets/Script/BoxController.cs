using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BoxController : MonoBehaviour
{

    private GameObject gripper;
    private bool hasCollidedWithFloor = false;
    public void SetGripper(GameObject gripper)
    {
        this.gripper = gripper;
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.gameObject.CompareTag("Floor") && !transform.name.Contains("Weight"))
        {
            if (!hasCollidedWithFloor)
            {
                ReleaseBox();
                hasCollidedWithFloor = false;
            }
            else
            {
                hasCollidedWithFloor = true;
            }
        }
        else if (hit.gameObject.CompareTag("Box") || transform.name.Contains("Weight") && !(transform.position.x >= 8.2f && transform.position.x <= 9.6f))
        {
            AttachBox(hit.gameObject);

            if (transform.name.Contains("Weight")) ReleaseBox();
        }
    }

    void AttachBox(GameObject otherBox)
    {
        if (transform.parent == null && otherBox.transform.position.y > transform.position.y)
        {
            // Set the current object as the parent of the other box
            transform.SetParent(otherBox.transform);

            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }

    void ReleaseBox()
    {
        if (gripper != null)
        {
            gripper.gameObject.SendMessage("ReleaseBox", transform.gameObject);
        }
    }
}
