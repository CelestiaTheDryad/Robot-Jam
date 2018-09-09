using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyNotifier : MonoBehaviour
{
    public GameObject fairy;
    private FairyAI fairyAI;

    // Use this for initialization
    void Start()
    {
        fairyAI = fairy.GetComponent(typeof(FairyAI)) as FairyAI;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Waterable waterable = other.gameObject.GetComponent(typeof(Waterable)) as Waterable;
        if (waterable != null)
        {
            fairyAI.FoundWaterTarget(waterable);
        }
    }
}
