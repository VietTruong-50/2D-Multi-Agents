using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripperScaleController : MonoBehaviour
{
    public float speed = 1.2f;
    public float rayDist = 0.05f;
    public Transform grabObject;
    public Transform boxHolder;
    public Direction direction;

    private bool isGrabbing = false;
    private bool isRelease = false;
    private bool isStop = false;
    private bool hasReleased = false;
    private bool isArmComplete = false;

    public GameObject magnet;

    void Update()
    {
        
    }

    public void DoStuff(float stopPosition, float minPosition, GameObject requireBox)
    {
        magnet = transform.GetChild(0).GetChild(0).gameObject;

        if (requireBox != null)
        {
            float boxPositionX = requireBox.transform.position.x;

            if (!isStop && !isGrabbing && hasReleased == false)
            {
                UpdateMovement(boxPositionX);
            }

            if (Mathf.Abs(transform.position.x - boxPositionX) < 0.003f && hasReleased == false)
            {
                UpdateWhenCloseToBox(stopPosition, minPosition);
            }
            else if (hasReleased)
            {
                UpdateWhenReleased(minPosition, stopPosition);
            }
        }
    }

    void UpdateMovement(float boxPositionX)
    {
        if (transform.position.x > boxPositionX)
        {
            MoveLeft();
        }
        else if (transform.position.x < boxPositionX)
        {
            MoveRight();
        }
    }

    void UpdateWhenCloseToBox(float maxPosition, float minPosition)
    {
        if (!isGrabbing)
        {
            isStop = true;
            MagnetMoveDown();

            if (!isRelease)
            {
                GrabBox();
            }
        }
        else
        {
            HandleGrabbedBoxMovement(maxPosition, minPosition);
        }
    }

    void HandleGrabbedBoxMovement(float stopPosition, float minPosition)
    {
        if (transform.position.x < stopPosition && direction.Equals(Direction.Right))
        {
            HandleMovementTowardsStopPosition();
        }
        else if (transform.position.x > minPosition && direction.Equals(Direction.Left))
        {
            HandleMovementTowardsMinPosition();
        }
        else
        {
            isStop = true;
            MagnetMoveDown();
        }
    }

    void HandleMovementTowardsStopPosition()
    {
        isStop = false;

        if (magnet.transform.localPosition.y >= -0.9f)
        {
            magnet.transform.localPosition = new Vector3(magnet.transform.localPosition.x, -0.9f, magnet.transform.localPosition.z);

            if (!isStop)
            {
                MoveRight();
            }
        }
        else
        {
            MagnetMoveUp();
        }
    }

    void HandleMovementTowardsMinPosition()
    {
        isStop = false;

        if (magnet.transform.localPosition.y >= -0.9f)
        {
            magnet.transform.localPosition = new Vector3(magnet.transform.localPosition.x, -0.9f, magnet.transform.localPosition.z);

            if (!isStop)
            {
                MoveLeft();
            }
        }
        else
        {
            MagnetMoveUp();
        }
    }

    void UpdateWhenReleased(float minPosition, float maxPosition)
    {
        if (magnet.transform.localPosition.y >= -0.9f)
        {
            magnet.transform.localPosition = new Vector3(magnet.transform.localPosition.x, -0.9f, magnet.transform.localPosition.z);

            if (!isStop)
            {
                if (direction.Equals(Direction.Right))
                {
                    MoveLeft();

                    if (transform.position.x < minPosition)
                    {
                        isStop = true;
                        isArmComplete = true;
                    }
                }
                else
                {
                    MoveRight();

                    if (transform.position.x > maxPosition)
                    {
                        isStop = true;
                        isArmComplete = true;
                    }
                }
            }
        }
        else
        {
            MagnetMoveUp();
        }
    }


    public void MoveLeft()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    public void MoveRight()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    public void MagnetMoveDown()
    {
        magnet.transform.position += Vector3.down * Time.deltaTime * speed;
    }

    public void MagnetMoveUp()
    {
        magnet.transform.position += Vector3.up * Time.deltaTime * speed;
    }


    private void GrabBox()
    {
        RaycastHit2D grabCheck = Physics2D.Raycast(grabObject.position, Vector2.down * transform.localScale.y, rayDist);

        if (grabCheck.collider != null && grabCheck.collider.tag == "Box")
        {
            grabCheck.collider.gameObject.transform.parent = boxHolder;
            grabCheck.collider.gameObject.transform.position = boxHolder.position;
            grabCheck.collider.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            isGrabbing = true;
            isRelease = false;
            hasReleased = false;
        }
    }

    private void ReleaseBox(GameObject childBox)
    {

        if (isGrabbing)
        {
            Transform grabbedBox = boxHolder.GetChild(0);
            grabbedBox.SetParent(null);

            Rigidbody2D boxRigidbody = grabbedBox.GetComponent<Rigidbody2D>();
            if (boxRigidbody != null)
            {
                boxRigidbody.isKinematic = false;
            }

            isRelease = true;
            isGrabbing = false;
            hasReleased = true;
            isStop = false;
        }
    }

    public bool IsRelease()
    {
        return isRelease;   
    }

    public bool IsArmComplete()
    {
        return isArmComplete;
    }

    public void SetDirection(Direction direction)
    {
        this.direction = direction;
    }

    public void Reset()
    {
        isGrabbing = false;
        isRelease = false;
        isStop = false;
        hasReleased = false;
        isArmComplete = false;
    }
}

