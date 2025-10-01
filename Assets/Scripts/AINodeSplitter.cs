using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sanicball
{
    public class AINodeSplitter : AINode
    {
        public List<AINodeSplitterTarget> targets = new();

        public override AINode NextNode
        {
            get
            {
                return targets.RandomElementByWeight(e=>e.Weight).Node;
            }
        }

        public override void AddNextNode(AINode newNode)
        {
            targets.Add(new(newNode, 1));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(transform.position, 3f);

            foreach (AINodeSplitterTarget target in targets)
            {
                if (target.Node != null)
                {
                    Gizmos.DrawLine(transform.position, target.Node.transform.position);
                }
            }
        }
    }

    [System.Serializable]
    public struct AINodeSplitterTarget
    {
        public AINode Node;
        public int Weight;

        public AINodeSplitterTarget(AINode node = null, int weight = 1)
        {
            Node = node;
            Weight = weight;
        }
    }
}