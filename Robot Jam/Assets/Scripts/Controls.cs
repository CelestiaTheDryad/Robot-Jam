using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    // Get the left / right control amount
    public float GetLeftRightMovement()
    {
        return Input.GetAxis("Horizontal");
    }

    public bool IsJumping()
    {
        return (Input.GetKey("w") || Input.GetKey("space"));
    }

    public bool DrinkWater()
    {
        return (Input.GetKey("q") || Input.GetKey("e"));
    }
    
    public bool GiveWater()
    {
        return (Input.GetKey("f") || Input.GetKey("control"));
    }
    public bool GetWater()
    {
        return (Input.GetKey("g") || Input.GetKey("c"));
    }
}


 