using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snorlax.Prototype.Flocking
{ 
    [CreateAssetMenu(menuName = "Flock/Filter/Physics Filter")]
    public class PhysicsFilter : ContextFilter
    {
        public LayerMask physicsFilter;
        public float viewAngle;
        public float rayResolution;
        
        public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
        {
            List<Transform> filtered = new List<Transform>();
            RaycastHit hit;
            int StepCount = Mathf.RoundToInt(viewAngle * rayResolution);
            float stepAngleSize = viewAngle / StepCount;

            for (int i = 0; i <= StepCount; i++)
            {
                float angle = agent.transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                Vector3 dir = DirFromAngle(angle);
                Color color = Color.red;
                if (Physics.Raycast(agent.transform.position + Vector3.up, dir, out hit, agent.AgentFlock.SquareAvoidanceRadius, physicsFilter))
                {
                    if (!filtered.Contains(hit.transform))
                    {
                        filtered.Add(hit.transform);
                        color = Color.green;
                    }
                }
                //Debug.DrawLine(agent.transform.position, agent.transform.position + dir, color);
            }

            return filtered;
        }

        private Vector3 DirFromAngle(float angleInDegrees)
        {
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}
