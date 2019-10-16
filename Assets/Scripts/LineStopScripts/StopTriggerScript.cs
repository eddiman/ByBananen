using UnityEngine;

namespace LineStopScripts
{
    public class StopTriggerScript : MonoBehaviour
    {
        private void Start()
        {
            Transform parent = transform.parent;
            foreach (Transform child in parent)
            {
                if (child.CompareTag("TramStop"))
                {
                    parentStop = child.gameObject;
                }
            }
        }

        public bool stopIsLeftSide;
        public GameObject parentStop;
    }
}
