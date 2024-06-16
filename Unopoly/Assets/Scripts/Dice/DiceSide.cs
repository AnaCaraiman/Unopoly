using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DiceSide : MonoBehaviour
{
    bool onGround;
    public bool OnGround => onGround;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    public int SideValue()
    {
        int value = Int32.Parse(name);
        return value;

    }
}