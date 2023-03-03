using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snorlax.Prototype.Flocking
{
    [CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]
    public class AvoidanceBehavior : FilteredFlockBehavior
    {
        Vector3 currentvelocity;
        public float agentSmoothTime = 0.1f;
        public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

            Vector3 avoidanceMove = Vector3.zero;

            if (filteredContext.Count == 0)
            {
                return avoidanceMove;
            }

            int nAvoid = 0;

            foreach (Transform item in filteredContext)
            {
                Vector3 closestPoint = item.gameObject.GetComponent<Collider>().ClosestPoint(agent.transform.position);

                if (Vector3.SqrMagnitude(closestPoint - agent.transform.position) < flock.SquareAvoidanceRadius) //item.position
                {
                    nAvoid++;
                    avoidanceMove += (agent.transform.position - closestPoint);
                }
            }

            if(nAvoid > 0)
            {
                avoidanceMove /= nAvoid;
            }

            avoidanceMove = Vector3.SmoothDamp(agent.transform.forward, avoidanceMove, ref currentvelocity, agentSmoothTime);

            filteredContext.Clear();
            return avoidanceMove;
        }
    }
}
