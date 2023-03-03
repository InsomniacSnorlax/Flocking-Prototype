using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snorlax.Prototype.Flocking
{
    [CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]
    public class AlignmentBehavior : FilteredFlockBehavior
    {
        public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

            if (filteredContext.Count == 0)
            {
                return agent.transform.forward;
            }

            Vector3 alignmentMove = Vector3.zero;

            foreach (Transform item in filteredContext)
            {
                alignmentMove += item.transform.forward;
            }

            alignmentMove /= filteredContext.Count;

            filteredContext.Clear();

            return alignmentMove;
        }
    }
}
