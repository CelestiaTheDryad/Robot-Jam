using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton
public class MoodManager : MonoBehaviour
{
    protected static MoodManager __instance;
    public static MoodManager GetInstance()
    {
        if (__instance == null)
        {
            __instance = GameObject.FindObjectOfType<MoodManager>().GetComponent<MoodManager>();
            if (__instance == null)
            {
                Debug.LogError("MoodManager instance not found!");
            }
        }
        return __instance;
    }

    [SerializeField] protected Color aboveLightColor;
    [SerializeField] protected Color belowLightColor;
    [SerializeField] protected Color skyLightColor;
    [SerializeField] protected Light aboveLight;
    [SerializeField] protected Light belowLight;
    [SerializeField] protected float windbadVolume;
    [SerializeField] protected float sitarVolume;
    [SerializeField] protected float brookVolume;
    [SerializeField] protected float gardenVolume;
    [SerializeField] protected float riverVolume;
    [SerializeField] protected float heartVolume;
    [SerializeField] protected float windgoodVolume;
    [SerializeField] protected float chimesVolume;
    [SerializeField] protected AudioSource ambience0;
    [SerializeField] protected AudioSource ambience1;
    [SerializeField] protected AudioSource ambience2;
    [SerializeField] protected AudioSource ambience3;
    [SerializeField] protected AudioSource ambience4;
    [SerializeField] protected AudioSource ambience5;
    [SerializeField] protected AudioSource ambience6;
    [SerializeField] protected AudioSource ambience7;
    //[SerializeField] protected MoodState[] moodStates;
    [SerializeField] protected float goalMood;
    [SerializeField] protected float actualMood;
    [SerializeField] protected float blendMoodSpeed;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Do mood blending
        DoBlendMood();

        aboveLight.color = aboveLightColor;
        belowLight.color = belowLightColor;
        RenderSettings.ambientLight = skyLightColor;

        ambience0.volume = windbadVolume;
        ambience1.volume = sitarVolume;
        ambience2.volume = brookVolume;
        ambience3.volume = gardenVolume;
        ambience4.volume = riverVolume;
        ambience5.volume = heartVolume;
        ambience6.volume = windgoodVolume;
        ambience7.volume = chimesVolume;
    }

    public void SetMood (float _mood)
    {
        goalMood = Mathf.Clamp (_mood, 0f, 1.0f);
    }

    protected void DoBlendMood ()
    {

    }

    // Various init classes
    [System.Serializable]
    protected class MoodState
    {
        public string name;
        public float point;
        public Color skyLight;
        public Color aboveLight;
        public Color belowLight;
        public float ambience0;
        public float ambience1;
        public float ambience2;
        public float ambience3;
        public float ambience4;
    }
}
