using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float cameraDistance;
    public float playerDistance;
    public float baseSpeed;
    public float jumpSpeed;
    public float vineSpeed;
    public float waterPerJump;
    public float waterPerWalk;
    public float waterPerClimb;
    public float maxWater;
    public GameObject mainCamera;
    public Transform playerMesh;
    public Slider watermeter;
    public BottleController bottleHandler;
    public FairyAI fairy;
    public bool cameraSmoothing;
    
    private Rigidbody body;
    [SerializeField] protected PlayerAnimationController playerAnimationController;
    private bool hasJumped = false;
    private float timeSinceCentered = 0.0f;
    private float amountToDip = 0.0f;
    private float jumpLimiterRange = 0.31f;
    private float vineGrabRange = 0.5f;
    public float currentWater;
    private int currentBottles = 4;
    private bool hasDrunk = false;
    private bool goneToStart = false;

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
        currentWater = maxWater;
        if (playerAnimationController == null) {
            Debug.LogError("No playerAnimationController attached!");
        }
        watermeter.maxValue = maxWater;
        watermeter.value = maxWater;
        bottleHandler.setBottles(currentBottles);
    }

    void doMovement(float moveValue) {
        setWater(currentWater - waterPerWalk * Mathf.Abs(moveValue) * Time.deltaTime);
        Vector3 positionVector = new Vector3(transform.position.x, 0, transform.position.z);
        //use geometry to get angle
        float playerAngle = Mathf.Acos(positionVector.normalized.z);

        //adjust for the half of the circle missed by arccos
        if(transform.position.x > 0) {
            playerAngle = 2 * Mathf.PI - playerAngle;
        }

        Vector3 movementVector = new Vector3(-Mathf.Cos(playerAngle), 0, -Mathf.Sin(playerAngle)) * moveValue * baseSpeed;

        //if player is moving change facing
        //use player mesh facing instead of matser player facing
        //to keep camera from swinging around
        playerMesh.LookAt(playerMesh.position + movementVector);

        //keep falling velocity intact
        movementVector.y = body.velocity.y;

        //store downward velocity for camera dip
        if(body.velocity.y < -cameraDipThreshold) {
            amountToDip = Mathf.Max(body.velocity.y * cameraDipMultiplier, -cameraDipMaximum);
        }

        body.velocity = movementVector;

        // Do movement animation
        if (Mathf.Abs(moveValue) < .5f) {
            // If they're basically still, make them stand
            playerAnimationController.SetAction(PlayerAnimationController.ePlayerAction.Stand);
            playerAnimationController.SetRunState(false);
        }
        else {
            playerAnimationController.SetAction(PlayerAnimationController.ePlayerAction.Run);
            playerAnimationController.SetRunState(true);
        }
    }

    void doJump(float jumpValue) {
        //if player is on vine
        if (Physics.Raycast(transform.position, new Vector3(-transform.position.x, 0, -transform.position.z), vineGrabRange, 1 << 9)) {
            //turn gravity off to not fall
            body.useGravity = false;

            //face player towards wall if they're not on ground
            if(!Physics.Raycast(transform.position, new Vector3(0, -1, 0), jumpLimiterRange)) {
                playerMesh.LookAt(new Vector3(0, playerMesh.position.y, 0));
                setWater(currentWater - waterPerClimb * Time.deltaTime);
                playerAnimationController.SetAction(PlayerAnimationController.ePlayerAction.Climb);
                //playerAnimationController.ClimbingState(true);
            } else {
                //playerAnimationController.ClimbingState(false);
            }

            //player holding jump
            if (jumpValue > 0.5) {
                body.velocity = new Vector3(body.velocity.x, vineSpeed, body.velocity.z);
                hasJumped = true;
            }
            //if player is holding down
            else if (jumpValue < -0.5) {
                body.velocity = new Vector3(body.velocity.x, -vineSpeed, body.velocity.z);
            }
            //stay still on vine with no button press
            else {
                body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
            }

            //don't jump on vines
            return;
        }
        else {
            //turn gravity back on
            body.useGravity = true;
        }
        
        
        //if player is holding jump key
        if (jumpValue > 0.5) {
            //if player is on a jumpable surface
            if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), jumpLimiterRange) && !hasJumped) {
                setWater(currentWater - waterPerJump);
                //change vertical velocity to jump velocity
                body.velocity = new Vector3(body.velocity.x, jumpSpeed, body.velocity.z);
                hasJumped = true;
                playerAnimationController.SetAction(PlayerAnimationController.ePlayerAction.JumpUp);

            }
        }
        else {
            hasJumped = false;
        }
    }

    void setWater(float newWater) {
        if(newWater < 0.0f) {
            die();
        }
        else if (newWater > maxWater) {
            currentWater = maxWater;
        }
        else {
            currentWater = newWater;
            watermeter.value = currentWater;
            MoodManager.GetInstance().SetPlayerHealth(currentWater);
        }
    }

    void die() {
        Debug.Log("You Died");
    }
	
	// Update is called once per frame
	void Update () {
        float movement = Input.GetAxis("Horizontal");
        doMovement(movement);
        float jumpValue = Input.GetAxisRaw("Jump");
        doJump(jumpValue);

        //drink water
        if(Input.GetAxisRaw("water") < -0.1f) {
            if (currentBottles > 0 && !hasDrunk) {
                setWater(maxWater);
                hasDrunk = true;
                currentBottles -= 1;
                bottleHandler.setBottles(currentBottles);
            }
        }
        //give water to fairy
        else if(Input.GetAxisRaw("water") > 0.1f) {
            if (!hasDrunk && currentBottles > 0) {
                fairy.GiveWater(fairy.MaxWaterMeter);
                hasDrunk = true;
                currentBottles -= 1;
                bottleHandler.setBottles(currentBottles);
            }
        }
        else {
            hasDrunk = false;
        }

        // Notify the animator of our velocity
        playerAnimationController.InformVelocity(body.velocity);

        //lock player to circle
        Vector3 rawPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 clampedPosition = rawPosition.normalized * playerDistance;
        clampedPosition.y = transform.position.y;

        transform.position = clampedPosition;

        //move camera
        Vector3 cameraPosition = rawPosition.normalized * cameraDistance;
        cameraPosition.y = transform.position.y + 0.5f;
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

        if(transform.position.y > 77.4 && transform.position.x < -12.4 && !goneToStart) {
            Invoke("goToStart", 2);
            goneToStart = true;
        }
    }

    void goToStart() {
        transform.position = new Vector3(-2.6f, 12.34f, -41f);
    }

    private void OnTriggerEnter(Collider other) {
        //handle oasis here
    }
}
