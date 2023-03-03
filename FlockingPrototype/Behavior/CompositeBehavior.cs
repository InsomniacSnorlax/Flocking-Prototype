using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snorlax.Prototype.Flocking
{
    [CreateAssetMenu(menuName = "Flock/Behavior/CompositeBehavior")]
    public class CompositeBehavior : FlockBehavior
    {
        public FlockBehavior[] behaviors;

        public float[] Weights;
        public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            if(Weights.Length != behaviors.Length)
            {
                Debug.LogError("Data mismatch in " + name, this);
                return Vector3.zero;
            }

            Vector3 move = Vector3.zero;

            for (int i = 0; i < behaviors.Length; i++)
            {
                Vector3 partialMove = behaviors[i].CalculateMove(agent, context, flock);
                if (partialMove == Vector3.zero)
                {
                    continue;
                }
                partialMove *= Weights[i];

                if (partialMove.sqrMagnitude > Weights[i] * Weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= Weights[i];
                }

                move += partialMove;

            }
            
            return move;
        }
    }
}
