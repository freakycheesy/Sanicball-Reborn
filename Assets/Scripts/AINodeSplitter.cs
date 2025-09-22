using System.Collections.Generic;
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
                //Pick a random next node based on their weights
                List<int> choices = new List<int>();
                for (int i = 0; i < targets.Count; i++)
                {
                    for (int j = 0; j < targets[i].Weight; j++) choices.Add(i);
                }
                int randomChoice = Random.Range(0, choices.Count);
                return targets[choices[randomChoice]].Node;
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