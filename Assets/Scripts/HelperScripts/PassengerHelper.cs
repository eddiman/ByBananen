using UnityEngine;

namespace HelperScripts
{
    public class PassengerHelper : MonoBehaviour
    {
        public static GameObject CreatePassenger()
        {
            GameObject psngr = Instantiate(Resources.Load("Prefabs/Man2")) as GameObject;

            return psngr;
        }

    }
}
