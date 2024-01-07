using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction
{
    Left,
    Right
}

public class MainController : MonoBehaviour
{
    public GameObject[] arms;
    private int currentArmIndex = 0;
    private GameObject box;
    public GameObject floor;
    public GameObject scale;
    public GameObject horizontalAxis;
    public ArmEntity[] armEntities = new ArmEntity[4];
    public Direction direction = Direction.Right;
    private Direction previousDirection;
    private bool isDirectionChange = false;
    private bool isPause = false;
    private bool isWaiting = false;
    private HashSet<GameObject> boxList;

    public float requireMass;
    public GameObject scaleArm;
    public List<GameObject> weightList;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject e in weightList)
        {
            e.GetComponent<BoxController>().SetGripper(scaleArm);
        }

        box = GetNearestBoxPosition();

        previousDirection = direction;

        float scaleAxis = horizontalAxis.transform.localScale.y - 0.2f;

        for (int i = 0; i < armEntities.Length; i++)
        {
            float min_pos = -scaleAxis / 2 + (scaleAxis / armEntities.Length - 0.2f) * i;
            float max_pos = min_pos + scaleAxis / armEntities.Length - 0.2f;
            if (i == 0) min_pos += 0.8f;
            if (i == 1) max_pos = -0.42f;
            if (i == 3) max_pos -= 1.2f;

            armEntities[i] = new ArmEntity(i, max_pos, min_pos);
        }

        float smallestDistance = Mathf.Abs(box.transform.position.x - arms[currentArmIndex].transform.position.x);
        for (int i = 1; i < arms.Length; i++)
        {
            if (box.transform.position.x > arms[i].transform.position.x)
            {
                continue;
            }

            if (smallestDistance >= Mathf.Abs(box.transform.position.x - arms[i].transform.position.x))
            {
                smallestDistance = Mathf.Abs(box.transform.position.x - arms[i].transform.position.x);
                currentArmIndex = i;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool checkResult = false;

        ChangeDirection();
        PlayOrPause();

        //Get boxes
        HashSet<GameObject> boxes = GetBoxes(box);

        scale.GetComponent<ScaleCheck>().SetBoxes(boxes);
        scale.GetComponent<ScaleCheck>().SetRequireMass(requireMass);

        if (isWaiting == true)
        {
            checkResult = scale.GetComponent<ScaleCheck>().CheckTotalMass();

            if(checkResult == true)
            {
                isWaiting = false;
            }
        }

        if (isPause == false)
        {
            for (int i = 0; i < armEntities.Length; i++)
            {
                if (isWaiting == false)
                {
                    var arm = arms[currentArmIndex].GetComponent<GripperController2>();

                    if (i == currentArmIndex)
                    {
                        arm.SetDirection(direction);

                        if (boxes.Count > 0)
                        {
                            foreach (GameObject b in boxes)
                            {
                                b.GetComponent<BoxController>().SetGripper(arms[currentArmIndex]);
                                arm.DoStuff(armEntities[currentArmIndex].maxPosition, armEntities[currentArmIndex].minPosition, b);
                            }
                        }

                        bool isArmComplete = arm.IsArmComplete();

                        if (isArmComplete)
                        {
                            if (checkResult == false && (i == 1 || i == 2))
                            {
                                isWaiting = true;
                            }

                            if (direction.Equals(Direction.Left) && currentArmIndex == 0)
                            {
                                break;
                            }

                            if (direction.Equals(Direction.Right) && currentArmIndex == 3)
                            {
                                break;
                            }

                            currentArmIndex += direction == Direction.Right ? 1 : -1;
                        }
                    }
                    else if (i != currentArmIndex && isDirectionChange)
                    {
                        arm = arms[i].GetComponent<GripperController2>();
                        arm.Reset();
                    }
                }
                else
                {
                    var totalMass = scale.GetComponent<ScaleCheck>().GetTotalMass();

                    if (requireWeight == null)
                    {
                        requireWeight = FindRequireWeight(totalMass);
                    }


                    //Goi arm scale
                    scaleArm.GetComponent<GripperScaleController>().DoStuff(6.5f, box.transform.position.x, requireWeight);


                    if (scaleArm.GetComponent<GripperScaleController>().IsArmComplete())
                    {
                        isWaiting = false;

                        weightList.Remove(requireWeight);

                        if (checkResult == false && weightList.Count > 0)
                        {
                            scaleArm.GetComponent<GripperScaleController>().Reset();
                            isWaiting = true;
                            requireWeight = null;
                        }
                    }

                }
            }
        }

        isDirectionChange = direction != previousDirection;

        if (isDirectionChange)
        {
            // Direction has changed, do something
            Debug.Log("Direction changed from " + previousDirection + " to " + direction);

            // Update the previousDirection
            previousDirection = direction;
        }

        float boxPosX = box.transform.position.x;

        bool isMinPosition = Mathf.Abs(boxPosX - armEntities[0].minPosition) <= 0.05 && isDirectionChange;
        bool isMaxPosition = Mathf.Abs(boxPosX - armEntities[3].maxPosition) <= 0.05 && isDirectionChange;

        if (isMinPosition || isMaxPosition)
        {
            currentArmIndex = isMinPosition ? 0 : 3;

            ResetArms();
        }
    }

    HashSet<GameObject> GetBoxes(GameObject box)
    {
        HashSet<GameObject> boxes = new HashSet<GameObject>() { box };
        Transform parentTransform = box.transform.parent;
        while (parentTransform != null)
        {
            if (!parentTransform.gameObject.CompareTag("ArmPart"))
            {
                boxes.Add(parentTransform.gameObject);
            }
            parentTransform = parentTransform.parent;
        }
        return boxes;
    }


    void ResetArms()
    {
        foreach (GameObject arm in arms)
        {
            arm.GetComponent<GripperController2>().Reset();
        }
    }

    void ChangeDirection()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.direction = Direction.Right;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            this.direction = Direction.Left;
        }
    }

    void PlayOrPause()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.isPause = !isPause;
        }
    }

    private void SeeTheWorld()
    {
        boxList = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("Box"));

  /*      foreach (GameObject weight in boxList)
        {
            if (weight.name.Contains("Weight"))
            {
                weightList.Add(weight);
            }
        }*/
    }

    private GameObject GetNearestBoxPosition()
    {
        SeeTheWorld();

        float minDifferenceF = float.MaxValue;
        GameObject nearestBox = null;

        foreach (GameObject box in boxList)
        {
            float dtBetweenFloor = Mathf.Abs(floor.transform.position.y - box.transform.position.y);

            if (dtBetweenFloor < 1.8f && dtBetweenFloor < minDifferenceF)
            {
                minDifferenceF = dtBetweenFloor;
                nearestBox = box;
            }
        }

        return nearestBox;
    }

    private GameObject requireWeight = null;
    private GameObject FindRequireWeight(float currentTotalMass)
    {
        float minDifference = float.MaxValue;

        foreach (GameObject weight in weightList)
        {
            var weightMass = weight.GetComponent<Rigidbody2D>().mass;
            var predictMass = currentTotalMass + weightMass;
            var difference = Mathf.Abs(requireMass - predictMass);

            if (predictMass <= requireMass && difference <= minDifference)
            {
                minDifference = difference;
                requireWeight = weight;
            }
        }
        return requireWeight;
    }

}
