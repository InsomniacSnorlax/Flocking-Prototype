using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snorlax.Prototype.Flocking
{
    [CreateAssetMenu(menuName = "Flock/Behavior/SteeringCohesion")]
    public class SteeringCohesionBehavior : FilteredFlockBehavior
    {
        Vector3 currentvelocity;
        public float agentSmoothTime = 0.5f;

        public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

            if (filteredContext.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 cohesionMove = Vector3.zero;

            foreach (Transform item in filteredContext)
            {
                cohesionMove += item.position;
            }

            cohesionMove /= filteredContext.Count;

            cohesionMove -= agent.transform.position;
            if (float.IsNaN(currentvelocity.x) || float.IsNaN(currentvelocity.z)) currentvelocity = Vector3.zero;
            cohesionMove = Vector3.SmoothDamp(agent.transform.forward, cohesionMove, ref currentvelocity, agentSmoothTime);

            filteredContext.Clear();
            return cohesionMove;
        }
    }
}
