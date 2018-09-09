using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyAI : MonoBehaviour
{
    public GameObject Target;                       // Set this to the player
    public float VerticalOffset = 1.0f;             // How far above the player should the fairy float
    public float WaterableVerticalOffset = 0.5f;    // How far above the player should the fairy float
    public float Speed = 1.0f;                      // How fast the fairy is

    public float MeterNeededToWater = 1.0f;         // How much water meter is needed to water
    public float CurrentWaterMeter = 5.0f;          // Current water Level
    public float MaxWaterMeter = 5.0f;              // Max water level

    public float IdleUpRange = 0.6f;                // How far up and down to bob during idle
    public float IdleUpFreq = 6;                    // How often to bob up and down during idle
    public float IdleSideRange = 0.6f;              // How far side to side to bob during idle
    public float IdleSideFreq = 1.0f;               // How often to bob side to side during idle

    public float WateringTime;                      // How long in seconds it takes to water something

    public float MaxDistance = 0.5f;
    public float WaterMaxDistance = 0.3f;

    private Vector3 Position;
    private Vector3 Velocity;

    private List<Waterable> WaterTargets = new List<Waterable>();
    private float StartedWatering;

    private Vector3 Ideal;

    private STATES CurrentState;

    private float t;
    private float dt;


    enum STATES
    {
        FOLLOW,
        IDLE,
        MOVE_TO_WATER,
        WATER,
    }


    // Use this for initialization
    void Start()
    {
        CurrentState = STATES.FOLLOW;
        Position = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        dt = Time.deltaTime;
        t += dt;
        Position = gameObject.transform.position;
        Vector3 newPosition;
        switch (CurrentState)
        {
            case STATES.IDLE:
                //Debug.Log("Idle");
                if (CanWater())
                {
                    CurrentState = STATES.MOVE_TO_WATER;
                    goto case STATES.MOVE_TO_WATER;
                }
                if (Vector3.Distance(Position, GetIdealFollowPosition(Target, VerticalOffset)) < MaxDistance)
                {
                    newPosition = IdleRoutine(Position);
                    break;
                }
                CurrentState = STATES.FOLLOW;
                goto case STATES.FOLLOW;
            case STATES.FOLLOW:
                //Debug.Log("Follow");
                if (Vector3.Distance(Position, GetIdealFollowPosition(Target, VerticalOffset)) < MaxDistance)
                {
                    CurrentState = STATES.IDLE;
                    goto case STATES.IDLE;
                }
                newPosition = MoveToTarget(Target, VerticalOffset);
                Position = newPosition;
                break;
            case STATES.MOVE_TO_WATER:
                //Debug.Log("Moving To Water");
                if (Vector3.Distance(Position, GetIdealFollowPosition(WaterTargets[0].gameObject, WaterableVerticalOffset)) < WaterMaxDistance)
                {
                    CurrentState = STATES.WATER;
                    StartWatering();
                    goto case STATES.WATER;
                }
                newPosition = MoveToTarget(WaterTargets[0].gameObject, WaterableVerticalOffset);
                Position = newPosition;
                break;
            case STATES.WATER:
                //Debug.Log("Watering");
                if (Time.fixedTime - StartedWatering > WateringTime)
                {
                    if (CanWater())
                    {
                        CurrentState = STATES.MOVE_TO_WATER;
                        goto case STATES.MOVE_TO_WATER;
                    }
                    else
                    {
                        CurrentState = STATES.FOLLOW;
                        goto case STATES.FOLLOW;
                    }
                }
                newPosition = Position; // Funky watering movement
                break;
            default:
                Debug.Log("Woops");
                newPosition = Position;
                break;
        }
        this.gameObject.transform.position = newPosition;
    }
    private void StartWatering()
    {
        Waterable waterable = WaterTargets[0];
        waterable.Watered = true;
        CurrentWaterMeter -= MeterNeededToWater;
        StartedWatering = Time.fixedTime;
    }

    private Vector3 MoveToTarget(GameObject target, float verticalOffset)
    {
        Ideal = GetIdealFollowPosition(target, verticalOffset);
        Vector3 Direction = Ideal - Position;
        return Position + Direction * dt * Speed;
    }

    private Vector3 GetIdealFollowPosition(GameObject target, float verticalOffset)
    {
        Vector3 targetPosition = target.transform.position;
        return new Vector3(targetPosition.x, targetPosition.y + verticalOffset, targetPosition.z);
    }

    private Vector3 IdleRoutine(Vector3 currentPosition)
    {
        Vector3 ideal = Ideal;

        float newY = ideal.y + (Mathf.Sin(t * IdleUpFreq) * IdleUpRange);

        Vector3 newPoint = new Vector3(ideal.x, newY, ideal.z);

        Vector3 normal = Vector3.Cross(Camera.main.transform.position, new Vector3(0, 1, 0)).normalized;
        newPoint += normal * Mathf.Sin(t * IdleSideFreq) * IdleSideRange;

        Vector3 direction = newPoint - currentPosition;
        return Position + direction * dt * Speed;
    }

    private bool CanWater()
    {
        if (WaterTargets.Count > 0)
        {
            Waterable nextWaterable = WaterTargets[0];
            if (WaterTargets[0].Watered == true)
            {
                WaterTargets.RemoveAt(0);
                return CanWater();
            }
            return CurrentWaterMeter >= nextWaterable.WateringCost;
        }
        return false;
    }

    public void GiveWater(float waterAmount)
    {
        CurrentWaterMeter = Mathf.Max(MaxWaterMeter, CurrentWaterMeter + waterAmount);
    }

    /**
     * Used to queue a Waterable object for the fairy to go and water.
     **/
    public void FoundWaterTarget(Waterable waterable)
    {
        WaterTargets.Add(waterable);
    }
}
