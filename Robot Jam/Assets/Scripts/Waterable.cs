using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterable : MonoBehaviour
{
    public static List<Waterable> allWaterables;
    public bool Watered = false;
    public float WateringCost = 1.0f;
    public float WateringTimeNecessary = 2f;    // The amount of time the fairy has to "work it" before the tree responds
    [SerializeField] float timeBeforeAnimationStarts = 1f;
    [SerializeField] float timeBeforeSwappingMeshes = 1f;
    public float VerticalOffset = 0.5f;
    [SerializeField] protected GameObject unwateredObject;
    [SerializeField] protected GameObject wateredObject;
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
        if (unwateredObject != null) {
            unwateredObject.SetActive(!_watered);
        }
        if (wateredObject != null) {
            wateredObject.SetActive(_watered);
        }
    }

    public void Water()
    {
        // Set state to true
        Watered = true;

        // Wait a certain amount of time before playing the animations
        TimedEventManager.GetInstance().AddTimedEvent(timeBeforeAnimationStarts, () => {
            // Play watering animation
            foreach (ParticleSystem i in wateredParticles) {
                i.Play();
            }

            // Delay a certain amount of time before swapping meshes
            TimedEventManager.GetInstance().AddTimedEvent(timeBeforeAnimationStarts, () => {
                EnableRendering(Watered);
            });

            // Play sound
            //AudioManager.PlaySound("Water");
        });
    }
}
