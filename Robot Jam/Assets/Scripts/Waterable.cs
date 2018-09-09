using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterable : MonoBehaviour
{
    public static List<Waterable> allWaterables;
    public bool Watered = false;
    public float WateringCost = 1.0f;
    public float VerticalOffset = 0.5f;
    private Color green = new Color(0, 1, 0);
    [SerializeField] protected Renderer unwateredStateRenderer;
    [SerializeField] protected Renderer wateredStateRenderer;
    [SerializeField] ParticleSystem[] wateredParticles;

    // Use this for initialization
    void Start()
    {
        if (allWaterables == null) {
            allWaterables = new List<Waterable>();
        }
        allWaterables.Add(this);
        EnableRendering (Watered);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void EnableRendering (bool _watered) {
        if (unwateredStateRenderer != null) {
            unwateredStateRenderer.enabled = !_watered;
        }
        if (wateredStateRenderer != null) {
            wateredStateRenderer.enabled = _watered;
        }
    }

    public void Water()
    {
        // Play watering animation
        foreach (ParticleSystem i in wateredParticles) 
        {
            i.Play ();
            // @TODO: Build delay period for enabling rendering based on particle state
        }

        // Play sound
        //AudioManager.PlaySound("Water");

        // Set state to true
        Watered = true;

        // Rendering
        EnableRendering(Watered);

        Material m = gameObject.GetComponent<Renderer>().material;
        m.color = green;
    }
}
