using System.Collections.Generic;
using System.IO.Compression;
using ControllerScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TramScripts
{
    public class TramTransporting : MonoBehaviour
    {
        public bool stopIsOnRightSide;
        public bool stopIsOnLeftSide;

        public bool unloadingPassengers;
        public bool loadingPassengers;

        public bool isFull;
        public bool isEmpty;
        
        public List<Transform> listOfPassengers;
        public int tramPassengerCounter;

        // Start is called before the first frame update
        void Start()
        {
            listOfPassengers = new List<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void CheckPassengerCapacity()
        {
            //This shit is fucking wack
            if (!stopIsOnRightSide && !stopIsOnLeftSide) return;
            var tramIsOpen = SceneGlobals.currentTramMechanics.isOpenLeftSide ||
                             SceneGlobals.currentTramMechanics.isOpenRightSide;
            if (!tramIsOpen)
            {
                if (SceneGlobals.currentTramData.noOfPassengers < SceneGlobals.currentTramData.maxNoOfPassengers)
                {
                    unloadingPassengers = false;
                    loadingPassengers = false;
                } else if (SceneGlobals.currentTramData.noOfPassengers == SceneGlobals.currentTramData.maxNoOfPassengers)
                {
                    unloadingPassengers = false;
                    loadingPassengers = false;
                }
            } else
            {
                if (SceneGlobals.currentTramData.noOfPassengers < SceneGlobals.currentTramData.maxNoOfPassengers)
                {
                    unloadingPassengers = false;
                    loadingPassengers = true;
                } else if (SceneGlobals.currentTramData.noOfPassengers == SceneGlobals.currentTramData.maxNoOfPassengers)
                {
                    unloadingPassengers = true;
                    loadingPassengers = false;
                }
            }
        }

    }
}