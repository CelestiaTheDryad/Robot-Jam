using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public CameraPoint[] cameraPoints;
    public int cameraDistance;
    public float baseSpeed;
    public GameObject mainCamera;

    private float currentAngle = 0;
    private CameraPoint currentViewpoint;
    private bool doMovement = true;
    private float changePerTick;

    // Use this for initialization
    void Start() {
        if(cameraPoints.Length > 0) {
            processNewCameraPoint(cameraPoints[0]);
        }
        else {
            Debug.Log("No camera points inputted, movement considered invalid.");
            doMovement = false;
        }

    }

    // Update is called once per frame
    void Update() {
        if(!doMovement) {
            return;
        }

        //get player movement
        float movement = Input.GetAxisRaw("Horizontal");
        float newAngle = currentAngle + changePerTick * movement;
        newAngle = Mathf.Clamp(newAngle, currentViewpoint.startAngle, currentViewpoint.endAngle);

        Vector3 currentPos = transform.position;

        Vector3 newPos = new Vector3(currentViewpoint.center.x - currentViewpoint.gamePlayRadius * Mathf.Sin(newAngle), currentPos.y,
            currentViewpoint.center.z + currentViewpoint.gamePlayRadius * Mathf.Cos(newAngle));

        transform.position = newPos;
        currentAngle = newAngle;


    }

    //called once per frame, after all update() calls
    void LateUpdate() {
        if(!doMovement) {
            return;
        }
        //do camera movement (could probably just be in code after player movement)
        float viewDist = currentViewpoint.gamePlayRadius + (currentViewpoint.concave ? -cameraDistance : cameraDistance);
        Vector3 newPos = new Vector3(currentViewpoint.center.x - viewDist * Mathf.Sin(currentAngle), transform.position.y, currentViewpoint.center.z + viewDist * Mathf.Cos(currentAngle));

        mainCamera.transform.position = newPos;
        mainCamera.transform.LookAt(this.transform);
        //Debug.Log(newPos);
    }

    private void processNewCameraPoint(CameraPoint point) {
        currentViewpoint = point;
        currentAngle = currentViewpoint.startAngle;
        changePerTick = baseSpeed / currentViewpoint.gamePlayRadius;
    }
}

[System.Serializable]
public class CameraPoint {
    public Vector3 center;
    public float gamePlayRadius;
    public float startAngle;
    public float endAngle;
    public bool concave;
}
