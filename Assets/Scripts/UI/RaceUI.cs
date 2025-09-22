using System.Collections;
using System.Collections.Generic;
using Sanicball.Logic;
using UnityEngine;

namespace Sanicball.UI
{
    public class RaceUI : MonoBehaviour
    {
        [SerializeField]
        private PlayerPortrait portraitPrefab = null;

        [SerializeField]
        private Transform portraitContainer = null;

        public RaceManager TargetManager { get; set; }

        public static List<RaceUI> Instances;

        private void Start()
        {
            Instances.Add(this);
            for (int i = 0; i < TargetManager.PlayerCount; i++)
            {
                var p = Instantiate(portraitPrefab);
                p.transform.SetParent(portraitContainer, false);
                p.TargetPlayer = TargetManager[i];
            }
        }

        void OnDestroy()
        {
            Instances.Remove(this);
        }
    }
}