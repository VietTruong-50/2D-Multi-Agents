using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralController : MonoBehaviour
{
    public static event Action<string> ArmMessageEvent;

    public static void SendMessageToNextArm(string message)
    {
        ArmMessageEvent?.Invoke(message);
    }
}
