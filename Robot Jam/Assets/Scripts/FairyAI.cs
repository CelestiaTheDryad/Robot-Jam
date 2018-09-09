using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FairyAI : MonoBehaviour
{
    public GameObject Player;                       // Set this to the player

    public float MeterNeededToWater = 1.0f;         // How much water meter is needed to water
    public float CurrentWaterMeter = 5.0f;          // Current water Level
    public float MaxWaterMeter = 5.0f;              // Max water level
    public float WaterDrainRate = 0.0001f;          // Rate of water drain during player movement

    [System.Serializable]
    public struct MoveState
    {
        public float WaterPctNeeded;             // Percentage of water meter needed >= to be in this state.
        public float Speed;                      // How fast the fairy is
        public float IdleUpRange;                // How far up and down to bob during idle
        public float IdleUpFreq;                 // How often to bob up and down during idle
        public float IdleSideRange;              // How far side to side to bob during idle
        public float IdleSideFreq;               // How often to bob side to side during idle
        public float VerticalOffset;             // How far above the player should the fairy float
    }

    public MoveState HealthyState;
    public MoveState UnhealthyState;
    public MoveState DeathlyState;

    public float MaxDistance = 0.5f;                // How far the player has to be to transfer fairy from idle to follow
    public float WaterMaxDistance = 0.3f;           // How far the fairy has to be from a waterable to start watering it.

    private Vector3 Position;
    private Vector3 Velocity;

    private Rigidbody PlayerBody;

    private List<Waterable> WaterTargets = new List<Waterable>();
    private float WateringFinished;

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
        PlayerBody = Player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        dt = Time.deltaTime;
        t += dt;


        Vector3 playerVel = PlayerBody.velocity;
        if (playerVel.y < 0)
        {
            playerVel = new Vector3(playerVel.x, 0, playerVel.z);
        }
        CurrentWaterMeter = Mathf.Max(0, CurrentWaterMeter - (playerVel.magnitude * WaterDrainRate));
        MoveState moveState = GetMoveState();

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
                if (Vector3.Distance(Position, GetIdealFollowPosition(Player, moveState.VerticalOffset)) < MaxDistance)
                {
                    newPosition = IdleRoutine(Position, moveState);
                    break;
                }
                CurrentState = STATES.FOLLOW;
                goto case STATES.FOLLOW;
            case STATES.FOLLOW:
                //Debug.Log("Follow");
                if (CanWater())
                {
                    CurrentState = STATES.MOVE_TO_WATER;
                    goto case STATES.MOVE_TO_WATER;
                }
                if (Vector3.Distance(Position, GetIdealFollowPosition(Player, moveState.VerticalOffset)) < MaxDistance)
                {
                    CurrentState = STATES.IDLE;
                    goto case STATES.IDLE;
                }
                newPosition = MoveToTarget(Player, moveState, moveState.VerticalOffset);
                Position = newPosition;
                break;
            case STATES.MOVE_TO_WATER:
                //Debug.Log("Moving To Water");
                Waterable waterTarget = WaterTargets[0];
                float vOffset = waterTarget.VerticalOffset;
                if (Vector3.Distance(Position, GetIdealFollowPosition(waterTarget.gameObject, waterTarget.VerticalOffset)) < WaterMaxDistance)
                {
                    CurrentState = STATES.WATER;
                    StartWatering();
                    goto case STATES.WATER;
                }
                newPosition = MoveToTarget(waterTarget.gameObject, moveState, waterTarget.VerticalOffset);
                Position = newPosition;
                break;
            case STATES.WATER:
                //Debug.Log("Watering");
                if (Time.fixedTime >= WateringFinished)
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
                //Debug.Log("Woops");
                newPosition = Player.transform.position;
                break;
        }
        this.gameObject.transform.position = newPosition;
    }
    private void StartWatering()
    {
        Waterable waterable = WaterTargets[0];
        waterable.Water();
        CurrentWaterMeter = Mathf.Max(0, CurrentWaterMeter - waterable.WateringCost);
        WateringFinished = Time.fixedTime + waterable.WateringTimeNecessary;
    }

    private Vector3 MoveToTarget(GameObject target, MoveState moveState, float verticalOffset)
    {
        Ideal = GetIdealFollowPosition(target, verticalOffset);
        Vector3 Direction = Ideal - Position;
        return Position + Direction * dt * moveState.Speed;
    }

    private Vector3 GetIdealFollowPosition(GameObject target, float verticalOffset)
    {
        Vector3 targetPosition = target.transform.position;
        return new Vector3(targetPosition.x, targetPosition.y + verticalOffset, targetPosition.z);
    }

    private Vector3 IdleRoutine(Vector3 currentPosition, MoveState moveState)
    {
        Vector3 ideal = Ideal;

        float newY = ideal.y + (Mathf.Sin(t * moveState.IdleUpFreq) * moveState.IdleUpRange);

        Vector3 newPoint = new Vector3(ideal.x, newY, ideal.z);

        Vector3 normal = Vector3.Cross(Camera.main.transform.position - ideal, new Vector3(0, 1, 0)).normalized;
        newPoint += normal * Mathf.Sin(t * moveState.IdleSideFreq) * moveState.IdleSideRange;

        Vector3 direction = newPoint - currentPosition;
        return Position + direction * dt * moveState.Speed;
    }

    private bool CanWater()
    {
        if (WaterTargets.Count > 0)
        {
            Waterable nextWaterable = WaterTargets[0];
            if (nextWaterable.Watered == true || CurrentWaterMeter < MeterNeededToWater)
            {
                WaterTargets.RemoveAt(0);
                return CanWater();
            }
            return true;
        }
        return false;
    }

    /**
     * Used to give the fairy water
     **/
    public void GiveWater(float waterAmount)
    {
        CurrentWaterMeter = Mathf.Min(MaxWaterMeter, CurrentWaterMeter + waterAmount);
    }

    /**
     * Used to queue a Waterable object for the fairy to go and water.
     **/
    public void FoundWaterTarget(Waterable waterable)
    {
        WaterTargets.Add(waterable);
    }

    private MoveState GetMoveState()
    {
        float waterPercentage = CurrentWaterMeter / MaxWaterMeter;
        if (waterPercentage >= HealthyState.WaterPctNeeded)
        {
            return HealthyState;
        }
        else if (waterPercentage >= UnhealthyState.WaterPctNeeded)
        {
            return UnhealthyState;
        }
        else
        {
            return DeathlyState;
        }
    }

}
