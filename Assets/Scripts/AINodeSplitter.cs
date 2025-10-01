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

    public static class IEnumerableExtensions
    {

        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = (float)new System.Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;

            }

            return default(T);

        }

    }
}