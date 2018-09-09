using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterable : MonoBehaviour
{

    public bool Watered = false;
    public float WateringCost = 1.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private Color green = new Color(0, 1, 0);

    public void Water()
    {
        Watered = true;
        Material m = gameObject.GetComponent<Renderer>().material;
        m.color = green;
    }
}
