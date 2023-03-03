using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Jobs;

namespace Snorlax.Prototype.Flocking
{
    using QuadrantSystem;
    public class Flock : MonoBehaviour
    {
        public FlockAgent agentPrefab;
        List<FlockAgent> agents = new List<FlockAgent>();
        public FlockBehavior behavior;
        public LayerMask layerMask;
        public Transform seekTarget;

        public bool UsingQuad;

        [Range(10, 1000)] public int agentSize = 250;
        const float AgentDensity = 0.16f;

        int num = 0;
        Task[] tasks;
        [Range(1f, 100f)] public float agentSpeedMultiplier = 10f;
        [Range(1f, 100f)] public float agentMaxSpeed = 5f;
        [Range(1f, 10f)] public float neighborRaidus = 1.5f;
        [Range(0f, 1f)] public float avoidanceRadiusMultiplier = 0.5f;

        public int CollisionCap;

        /// Object Pooling for dead to revive
        /// Leader that has navmesh agent and follows path to front
        /// Once Enemy in range switch to combat mode
        /// Agent leaves flock for combat mode
        /// Implement FSM
        /// Idle until enough allies respawn
        /// Fix up animator as horizontal doesn't work

        float squareMaxSpeed;
        float squareNeighborRadius;
        float squareAvoidanceRadius;

        QuadGrid grid;

        public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

        private void Start()
        {
            grid = new QuadGrid(10,10,1);
            squareMaxSpeed = agentMaxSpeed * agentMaxSpeed;
            squareNeighborRadius = neighborRaidus * neighborRaidus;
            squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

            for(int i  = 0; i < agentSize; i++)
            {
                Vector3 newPosition = Random.insideUnitSphere * agentSize * AgentDensity;
                //newPosition.y = 1.33f;
                newPosition.y = 0f;
                FlockAgent newAgent = Instantiate(agentPrefab, newPosition, Quaternion.Euler(Vector3.zero), transform);
                
                newAgent.name = "Agent " + i;
                newAgent.Init(this);
                agents.Add(newAgent);

                //QuadSystem.QuadData quadData = new QuadSystem.QuadData() { transform = newAgent.gameObject.transform, Key = 0 };
                //newAgent.quadData = quadData;
                QuadSystem.entities.Add(newAgent);
            }
            tasks = new Task[agents.Count];

            QuadSystem.Init(Radius);
        }

        private void Update()
        {
            if (UsingQuad)
                QuadSystem.OnUpdate();

            Move();
            //MoveAllflockAgents();
            //MoveTowardsTarget();
        }

        void MoveTowardsTarget()
        {
            foreach (FlockAgent agent in agents)
            {
                agent.NavAgent.SetDestination(seekTarget.transform.position);
            }
        }

        public async void MoveAllflockAgents()
        {
            num = 0;
            foreach (FlockAgent agent in agents)
            {
               tasks[num++] = MoveTask(agent);
            }

            await Task.WhenAll(tasks);
        }

        public async Task MoveTask(FlockAgent agent)
        {
            //List<Transform> context = GetNearbyObjects(agent);

            List<Transform> context = new List<Transform>();

            if (UsingQuad)
            {
                var temp = QuadSystem.FindTargets(agent, Radius);
                temp.ForEach(e => context.Add(e.targetTransform));
            }
            else
            {
                context = GetNearbyObjects(agent);
            }

            Vector3 move = behavior.CalculateMove(agent, context, this);
            move *= agentSpeedMultiplier;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * agentMaxSpeed;
            }
            move.y = 0;

            agent.Move(move);
            context.Clear();
            await Task.Yield();
        }

        public void Move()
        {
            foreach (FlockAgent agent in agents)
            {
                List<Transform> context = new List<Transform>();

                if (UsingQuad)
                {
                    var temp = QuadSystem.FindTargets(agent, Radius);
                    temp.ForEach(e => context.Add(e.targetTransform));
                }
                else
                {
                    context = GetNearbyObjects(agent);
                }


                Vector3 move = behavior.CalculateMove(agent, context, this);
                move *= agentSpeedMultiplier;
                if (move.sqrMagnitude > squareMaxSpeed)
                {
                    move = move.normalized * agentMaxSpeed;
                }
                move.y = 0;

                agent.Move(move);
                context.Clear();
            } 
        }

        List<Transform> list = new List<Transform>();
        public int Radius;

        /*
        private void FixedUpdate()
        {
            foreach (FlockAgent agent in agents)
            {
                if (agent.RigidBody == null)
                    break;
                agent.RigidBody.velocity = Vector3.ClampMagnitude(agent.RigidBody.velocity, 5f);
               
            }
        }*/



        private List<Transform> GetNearbyObjects(FlockAgent agent)
        {
            List<Transform> context = new List<Transform>();
            Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRaidus, layerMask, QueryTriggerInteraction.Ignore);
            foreach (Collider c in contextColliders)
            {
                if (c == agent.AgentCollider)
                {
                    continue;

                }

                if (context.Count > CollisionCap)
                {
                    break;
                }

                context.Add(c.transform);
            }

            return context;
        }

        private void OnValidate()
        {
            squareMaxSpeed = agentMaxSpeed * agentMaxSpeed;
            squareNeighborRadius = neighborRaidus * neighborRaidus;
            squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
        }

        #region Mouse Click Region
        PlayerControls pc;
        PlayerControls.CameraMovementActions cameraActions;

        private Vector2 StartMousePosition;
        Vector3[] points = new Vector3[100];
        private void ClickMove(InputAction.CallbackContext obj)
        {
            //int i = 0;
            StartMousePosition = Helper.MousePoisition();

            Ray ray = Helper.Camera.ScreenPointToRay(StartMousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {

                seekTarget.position = hit.point;

                //seekTarget.transform.position = hit.point;
            }
        }

        private void OnEnable()
        {
            if (pc == null)
            {
                pc = new PlayerControls();
            }

            cameraActions = pc.CameraMovement;
            cameraActions.LeftClick.started += ClickMove;

            pc.Enable();
        }

        private void OnDisable()
        {
            pc.Disable();

            cameraActions.LeftClick.started -= ClickMove;
        }

        #endregion
    }
}
