using Player.Movement;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class Player : Agent
    {
        public Vector3 startPos;
        public Quaternion startRot;
        public CountInBox nest;
        public Pickup pickup;
        public Transform center;

        public HeadMovement head;
        public MovementController body;

        private PlayerInput playerInput;
        private InputAction look;
        private InputAction move;
        private InputAction use;
        private InputAction drop;

        private float maxBallReward = 0.5f;
        private float totalBallReward = 0;

        int group;

        public float DistanceToNest => Vector3.Distance(nest.transform.position, transform.position);
        public float DistanceToCenter => Vector3.Distance(center.position, transform.position);

        public override void Initialize()
        {
            startPos = transform.position;
            center = GameObject.FindGameObjectWithTag("Center").transform;
            startRot = transform.rotation;
            group = GetComponent<BehaviorParameters>().TeamId;
            if (playerInput == null)
            {
                playerInput = FindObjectOfType<PlayerInput>();
                if (playerInput == null)
                    throw new MissingComponentException("Missing a PlayerInput componenet");
                look = playerInput.actions["Look"]; // This is our Vector2 with the mouse movement
                move = playerInput.actions["Move"];
                use = playerInput.actions["Use"];
                drop = playerInput.actions["Drop"];
            }
            nest.onBallGain.AddListener(() => {
                if (nest.lastBallOwner == group) 
                {
                    AddReward(1f);
                }
                if (nest.Count > 15) 
                {
                    onEpisodeEnd();
                    Debug.Log("Win");
                }
            });
            nest.onBallLoss.AddListener(() => {
                AddReward(-0.1f);
            });
            foreach (var well in Knowledge.boxes) 
            {
                if (well != nest) 
                {
                    well.onBallGain.AddListener(() => 
                    {
                        AddReward(-0.3f);
                    });
                    well.onBallLoss.AddListener(() =>
                    {
                        AddReward(0.3f);
                    });
                }
            }
            FindObjectOfType<Timer>().onTimeOut.AddListener(() => {
                onEpisodeEnd();
                Debug.Log("Out of Time");
            });
            pickup.BallPickup.AddListener(() => {
                if (totalBallReward >= maxBallReward)
                    return;
                totalBallReward += 0.1f;
                AddReward(0.1f);
            });
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(pickup.BallInHand);
            sensor.AddObservation(pickup.BallInReach);
            sensor.AddObservation(DistanceToNest);
            sensor.AddObservation(DistanceToCenter);
            float angle = transform.rotation.eulerAngles.y + startRot.eulerAngles.y;
            if (angle >= 360) 
            {
                angle -= 360;
            }
            if (angle < 0)
            {
                angle += 360;
            }
            sensor.AddObservation(angle);
        }

        public void onEpisodeEnd() 
        {
            if (Knowledge.winnerNest() == nest) 
            {
                AddReward(FindObjectOfType<Timer>().timeLeft / (FindObjectOfType<Timer>().MaxTime / 3));
            }
            EndEpisode();
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var l = new Vector2(Mathf.Clamp(actions.ContinuousActions[0], -1, 1),
                Mathf.Clamp(actions.ContinuousActions[1], -1, 1));
            var m = new Vector2(Mathf.Clamp(actions.ContinuousActions[2], -1, 1),
                Mathf.Clamp(actions.ContinuousActions[3], -1, 1));
            var u = actions.DiscreteActions[0];
            var d = actions.DiscreteActions[1];

            head.MouseInput(l);
            body.SetMovement(m);
            
            pickup.SetDrop(d);
            pickup.SetUse(u);
        }

        public override void OnEpisodeBegin()
        {
            totalBallReward = 0;
            body.Reset();
            transform.position = startPos;
            transform.rotation = startRot;
            body.Run();
            pickup.Reload();
            FindObjectOfType<SummonBall>().Reset();
            foreach (var Box in FindObjectsOfType<CountInBox>()) 
            {
                Box.Reset();
            }
            FindObjectOfType<Timer>().Restart();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            List<float> continuous = new List<float>();
            List<int> discrete = new List<int>();

            var l = look.ReadValue<Vector2>();
            continuous.Add(l.x);
            continuous.Add(l.y);
            var m = move.ReadValue<Vector2>();
            continuous.Add(m.x);
            continuous.Add(m.y);

            discrete.Add(use.ReadValue<float>() > 0.5f ? 1 : 0);
            discrete.Add(drop.ReadValue<float>() > 0.5f ? 1 : 0);

            actionsOut.ContinuousActions.Clear();
            actionsOut.DiscreteActions.Clear();

            var con = actionsOut.ContinuousActions;
            for(int i = 0; i < continuous.Count; i++)
                con[i] = continuous[i];

            var dis = actionsOut.DiscreteActions;
            for (int i = 0;i < discrete.Count; i++)
                dis[i] = discrete[i];
        }
    }
}