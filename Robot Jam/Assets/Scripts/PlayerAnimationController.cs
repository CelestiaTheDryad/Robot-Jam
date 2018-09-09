using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    [SerializeField] protected AnimationController animationController;
    [SerializeField] protected Animator theAnimator;

	// Use this for initialization
	void Start () {
        TimedEventManager.GetInstance().AddTimedEvent(2f, () => {
            Debug.Log("Attempt transition!");
            theAnimator.SetTrigger("startWalking");
        });
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(TimedEventManager.GetInstance());
	}
}
