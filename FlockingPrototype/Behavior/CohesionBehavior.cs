using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snorlax.Prototype.Flocking
{
    [CreateAssetMenu(menuName ="Flock/Behavior/Cohesion")]
    public class CohesionBehavior : FilteredFlockBehavior
    {
        public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

            Vector3 cohesionMove = Vector3.zero;

            if (filteredContext.Count == 0)
            {
                return cohesionMove;
            }

            foreach (Transform item in filteredContext)
            {
                cohesionMove += item.position;
            }

            cohesionMove /= filteredContext.Count;

            cohesionMove -= agent.transform.position;

            return cohesionMove;
        }
    }
}
