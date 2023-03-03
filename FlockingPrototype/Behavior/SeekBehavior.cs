using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snorlax.Prototype.Flocking
{
    [CreateAssetMenu(menuName = "Flock/Behavior/Seek")]

    public class SeekBehavior : FlockBehavior
    {
        Vector3 currentvelocity;
        public float agentSmoothTime = 0.5f;
        public float agentSlerpAmount = 0.5f;
        [Tooltip("True uses Slerp, False uses Smoothdamp")]
        public bool vector3Method;
        public LayerMask physicsFilter;

        public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            /*
            List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

            if (filteredContext.Count == 0)
            {
                return Vector3.zero;
            }

            filteredContext.Clear();*/

            Vector3 seekMove = Vector3.zero;

            if(flock.seekTarget == null)
            {
                return seekMove;
            }

            seekMove += flock.seekTarget.transform.position - agent.transform.position;

            if (vector3Method)
            {
                seekMove = Vector3.Slerp(agent.transform.forward, seekMove, agentSlerpAmount);
            }
            else
            {
                seekMove = Vector3.SmoothDamp(agent.transform.forward, seekMove, ref currentvelocity, agentSmoothTime);
            }

            return seekMove;
        }
    }
}
