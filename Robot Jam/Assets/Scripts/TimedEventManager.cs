using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEventManager : MonoBehaviour {
    protected static TimedEventManager __instance;
    protected Dictionary<float, Action> __timedfunctions;

    protected void Start() {
        __instance = this;
    }

    // Update is called once per frame
    void Update () {
        foreach (KeyValuePair<float, Action> i in __timedfunctions) {
            if (Time.time >= i.Key) {
                i.Value();
                // @TODO: Broken, need to fix
            }
        }
	}

    public static TimedEventManager GetInstance () {
        if (__instance == null) {
            Debug.LogError("TimedEventManager instance doesn't exist");
        }
        return __instance;
    }

    // Adds a timed event set to go off in _inSeconds seconds
    public void AddTimedEvent (float _inSeconds, Action _event) {
        // @TODO: dictionary won't work lol
    }
}
