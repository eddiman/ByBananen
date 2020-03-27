using TramScripts;
using UnityEngine;

namespace ControllerScripts
{
    public class TramController : MonoBehaviour
    {
        public GameObject currentTram;

        public GameObject GetCurrentTram()
        {
            return currentTram;
        }
        public void SetCurrentTram(GameObject newTram)
        {
            currentTram = newTram;
        }

        public TramMechanics GetTramMechanics()
        {
            return currentTram.GetComponent<TramMechanics>();
        }
        public TramData GetTramData()
        {
            return currentTram.GetComponent<TramData>();
        }
        public TramTransporting GetTramTransporting()
        {
            return currentTram.GetComponent<TramTransporting>();
        }
        public TramEngine GetTramEngine()
        {
            return currentTram.GetComponent<TramEngine>();
        }

    }
}
