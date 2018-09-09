using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

    [SerializeField] protected Animator theAnimator;
    protected ePlayerAction currentAction;

	// Use this for initialization
	void Start () {
        TimedEventManager.GetInstance().AddTimedEvent(.5f, () => {
            theAnimator.SetTrigger("doStand");
        });
	}
	
	// Update is called once per frame
	void Update () {
	}


    public void InformVelocity (Vector3 velocity) {
        theAnimator.SetFloat("yVelocity", velocity.y);
    }

    public void SetRunState (bool running) {
        theAnimator.SetBool("isRunning", running);
    }

    public void SetAction(ePlayerAction _action) {
        if (_action != currentAction) {
            switch (_action) {
                case ePlayerAction.Stand:
                    break;
                    theAnimator.SetTrigger("doStand");
                    break;
                case ePlayerAction.JumpUp:
                    theAnimator.SetTrigger("StartJump");
                    Debug.Log("Start Jump!");
                    break;
                default:
                    break;
            }
            currentAction = _action;
        }
    }

    public enum ePlayerAction {
        Stand, Run, JumpUp, Fall, Drink, Share, Die
    }
}
