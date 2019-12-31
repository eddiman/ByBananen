using UnityEngine;

namespace ControllerScripts
{
    public class PassengerController : MonoBehaviour
    {
        public int passengersTransported = 0;


        public void addPassengersTransported()
        {
            passengersTransported++;
        }
    }
}
