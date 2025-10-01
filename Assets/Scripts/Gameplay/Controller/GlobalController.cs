using UnityEngine;

namespace Sanicball.Gameplay
{
    public class GlobalController : MonoBehaviour
    {
        public static GlobalController Instance;
        void Start()
        {
            if (Instance)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        void Update()
        {
            foreach (var controller in LocalController.Controllers)
                controller.OnUpdate();
        }
        void FixedUpdate()
        {
            foreach (var controller in LocalController.Controllers)
                controller.OnFixedUpdate();
        }
    }
}