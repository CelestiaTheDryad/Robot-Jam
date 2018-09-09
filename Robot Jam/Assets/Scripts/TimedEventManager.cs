using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEventManager : MonoBehaviour {
    protected static TimedEventManager __instance;
    protected List<TimedEvent> __timedEvents;

    protected void Start() {
        __instance = this;
        __timedEvents = new List<TimedEvent>();
    }

    // Update is called once per frame
    void Update () {
        int count = __timedEvents.Count;
        for (int i = count - 1; i >= 0; i--) {
            if (__timedEvents[i].time <= Time.time) {
                __timedEvents[i].action();
                __timedEvents.RemoveAt(i);
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
        __timedEvents.Add(new TimedEvent(Time.time + _inSeconds, _event));
    }

    protected class TimedEvent {
        public float time;
        public Action action;
        public TimedEvent (float _time, Action _action) {
            time = _time;
            action = _action;
        }
    }
}
