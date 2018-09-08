using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float cameraDistance;
    public float playerDistance;
    public float baseSpeed;
    public float jumpSpeed;
    public float jumpLimiterRange;
    public GameObject mainCamera;
    public bool cameraSmoothing;
    
    private Rigidbody body;
    private bool hasJumped = false;
    private float timeSinceCentered = 0.0f;
    private float amountToDip = 0.0f;

    //smoothing config values
    private float smoothCameraBaseSpeed = 0.1f;
    private float smoothCameraMaxSpeed = 0.85f;
    private float cameraDipDampening = 0.9f;
    private float cameraDipThreshold = 1.0f;
    private float cameraDipMultiplier = 0.125f;
    private float cameraDipMaximum = 2.0f;

    // Use this for initialization
    void Start () {
		body = GetComponent<Rigidbody>();
	}

    void doMovement(float moveValue) {
        Vector3 positionVector = new Vector3(transform.position.x, 0, transform.position.z);
        float playerAngle = Vector3.Angle(new Vector3(1, 0, 0), positionVector);
        Vector3 movementVector = new Vector3(baseSpeed * Mathf.Sin(playerAngle * Mathf.Deg2Rad), 0, baseSpeed * Mathf.Cos(playerAngle * Mathf.Deg2Rad)) * moveValue;
        //keep falling velocity intact
        movementVector.y = body.velocity.y;

        //store downward velocity for camera dip
        if(body.velocity.y < -cameraDipThreshold) {
            //Debug.Log(body.velocity.y);
            amountToDip = Mathf.Max(body.velocity.y * cameraDipMultiplier, -cameraDipMaximum);
        }

        body.velocity = movementVector;
    }

    void doJump(float jumpValue) {
        //check for player able to jump here

        if (jumpValue > 0.5) {
            if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), jumpLimiterRange) && !hasJumped) {
                //change vertical velocity to jump velocity
                body.velocity = new Vector3(body.velocity.x, jumpSpeed, body.velocity.z);
                hasJumped = true;
            }
        }
        else {
            hasJumped = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        float movement = Input.GetAxis("Horizontal");
        doMovement(movement);
        float jumpValue = Input.GetAxisRaw("Jump");
        doJump(jumpValue);

        //lock player to circle
        Vector3 rawPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 clampedPosition = rawPosition.normalized * playerDistance;
        clampedPosition.y = transform.position.y;

        transform.position = clampedPosition;

        //move camera
        Vector3 cameraPosition = rawPosition.normalized * cameraDistance;
        cameraPosition.y = transform.position.y;
        if (cameraSmoothing) {

            //artificially move camera further down to have a dip when the player lands
            if(Mathf.Abs(body.velocity.y) < cameraDipThreshold) {
                cameraPosition.y += amountToDip;
                amountToDip *= cameraDipDampening;
            }
            else if(body.velocity.y > cameraDipThreshold) {
                amountToDip = 0.0f;
            }
            float distToMove = (cameraPosition - mainCamera.transform.position).magnitude;

            if(distToMove > 1) {
                timeSinceCentered += Time.deltaTime;
            }
            else {
                timeSinceCentered = 0;
            }

            float movementPower = smoothCameraBaseSpeed + Mathf.Clamp(timeSinceCentered, 0, smoothCameraMaxSpeed - smoothCameraBaseSpeed);

            Vector3 newPos = (cameraPosition - mainCamera.transform.position) * movementPower + mainCamera.transform.position;

            mainCamera.transform.position = newPos;
            mainCamera.transform.LookAt(this.transform);
        }
        else {
            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.LookAt(this.transform);
        }
    }
}
