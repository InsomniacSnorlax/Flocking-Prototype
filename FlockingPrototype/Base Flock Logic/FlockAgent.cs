using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Snorlax.Prototype.Flocking
{
    using QuadrantSystem;
    [RequireComponent(typeof(Collider))]
    public class FlockAgent : MonoBehaviour, QuadData
    {
        Flock agentFlock;
        Rigidbody rigidBody;
        Animator anim;
        //public Rigidbody RigidBody { get { return rigidBody; } }
        public Flock AgentFlock { get { return agentFlock; } }
        Collider agentCollider;
        public Collider AgentCollider { get { return agentCollider; } }

        public Transform targetTransform { get; set; }
        public int quadID { get; set; }

        public NavMeshAgent NavAgent;

        private void Start()
        {
            agentCollider = GetComponent<Collider>();
            rigidBody = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            NavAgent = GetComponent<NavMeshAgent>();

            targetTransform = this.transform;
        }

        public void Init(Flock flock)
        {
            agentFlock = flock;
        }

        public void Move(Vector3 velocity)
        {
            curretTarget = velocity;
            transform.forward = velocity;
            transform.position += velocity * Time.deltaTime;

            float speed = Mathf.Clamp01(velocity.magnitude);
            float angle = Quaternion.FromToRotation(transform.forward, transform.position + velocity).eulerAngles.y;

            anim.SetFloat("Speed", speed);
            anim.SetFloat("Angle", angle);
        }

        Vector3 curretTarget;

        private void OnDrawGizmos()
        {

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + curretTarget);

        }
    }
}
