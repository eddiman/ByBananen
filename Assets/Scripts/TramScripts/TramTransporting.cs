using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using ControllerScripts;
using HelperScripts;
using LineStopScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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
        public int outgoingPassengerCounter;

        private bool _psngrIsEntering = true;
        private bool _psngrIsExiting = true;

        private float t = 0f;
        private float t2 = 0f;
        // Start is called before the first frame update
        void Start()
        {
            listOfPassengers = new List<Transform>();
        }

        private void UnloadPsngrsFromTram()
        {
            if (unloadingPassengers == false) return;

            unloadingPassengers = true;

            int unloadAmountThreshold = SceneGlobals.currentTramData.maxNoOfPassengers - SceneGlobals.currentTramData.noOfPassengers;
            int unloadAmount = Random.Range(0, SceneGlobals.currentTramData.noOfPassengers);
            _psngrIsExiting = true;
            if (_psngrIsExiting && unloadAmount > unloadAmountThreshold)
            {
                StartCoroutine(PassengerExitCoroutine());
            }
            else
            {
                unloadingPassengers = false;
            }


        }
        private IEnumerator PassengerExitCoroutine()
        {
            _psngrIsExiting = true;
            float passengerMoveTime1 = 0.7f;
            GameObject psngr = PassengerHelper.CreatePassenger();
            Vector3 position = transform.position;
            psngr.transform.position = position;
            Vector3 newPos = new Vector3(position.x + 1f, position.y - 0.5f, position.z - 3);

            while (t < passengerMoveTime1)
            {
                t += Time.deltaTime;
                psngr.transform.position = Vector3.Slerp(
                    new Vector3(position.x + 1.5f, position.y - 0.5f, position.z), newPos, t / passengerMoveTime1);
                yield return null;
            }
            if (psngr.transform. position == newPos)
            {
                _psngrIsExiting = false;
                Destroy(psngr);
                SceneGlobals.currentTramData.noOfPassengers--;
                outgoingPassengerCounter++;
                t = 0;
                UnloadPsngrsFromTram();
                yield return null;
            }
            yield return null;
            _psngrIsExiting = false;
        }
        private void LoadPsngrsOntoTram()
        {
            Debug.Log("loadpsngrrs start");
            if (loadingPassengers == false) return;

            loadingPassengers = true;
            Transform currentStop = SceneGlobals.currentTramData.currentStop;
            SubStopScript currentStopData =  currentStop.GetComponent<SubStopScript>();
            bool tramIsFull = SceneGlobals.currentTramData.noOfPassengers ==
                              SceneGlobals.currentTramData.maxNoOfPassengers;

            int loadAmountThreshold = currentStopData.waitingPassengers - currentStopData.waitingPassengers;
            int loadAmount = Random.Range(0, currentStopData.waitingPassengers);
            _psngrIsEntering = true;
            if (_psngrIsEntering && !tramIsFull && currentStopData.waitingPassengers != 0)
            {
                StartCoroutine(PassengerEnterCoroutine());
            }
            else
            {
                loadingPassengers = false;
            }
        }
        private IEnumerator PassengerEnterCoroutine()
        {
            float t1 = 0;
            float t2 = 0;
            _psngrIsEntering = true;
            float passengerMoveTimeStep1 = Random.Range(1f, 2.5f);
            float passengerMoveTimeStep2 = 1f;
            var stopParent = SceneGlobals.currentTramData.currentStop;
            Vector3 tramDoor = SceneGlobals.currentTram.transform.position;

            Vector3 psngrMoveStep1 = new Vector3(tramDoor.x + 4.5f, tramDoor.y - .5f,
                tramDoor.z - 3.5f ) ;
            Vector3 psngrMoveStep2 = new Vector3(tramDoor.x + 2, tramDoor.y - .5f,
                tramDoor.z + .5f);

                    Quaternion pointerTargetRotation = Quaternion.Euler(psngrMoveStep2);

            while (t2 < passengerMoveTimeStep2)
            {
                t2 += Time.deltaTime;

                stopParent.GetChild(0).gameObject.transform.position = Vector3.Lerp(
                    stopParent.GetChild(0).gameObject.transform.position,
                    psngrMoveStep2, t2 / passengerMoveTimeStep2/10);

                yield return null;

                while (t1 < passengerMoveTimeStep1)
                {
                    t1 += Time.deltaTime;

                    stopParent.GetChild(0).gameObject.transform.position = Vector3.Slerp(
                        stopParent.GetChild(0).gameObject.transform.position,
                        psngrMoveStep1, t1 / passengerMoveTimeStep1/10);
                    yield return null;

                }

                if (t2 > passengerMoveTimeStep2)
                {
                   // stopParent.GetChild(0).gameObject.transform.position = psngrMoveStep2;
                }
                if (t1 > passengerMoveTimeStep1 && t2 > passengerMoveTimeStep2)
                {

                    Debug.Log("loadpsngrrs end");

                    _psngrIsEntering = false;

                    SceneGlobals.currentTramData.noOfPassengers++;
                    SceneGlobals.currentTramData.currentStop.gameObject.GetComponent<SubStopScript>().subStopPsngrCounter++;
                    SceneGlobals.currentTramData.currentStop.gameObject.GetComponent<SubStopScript>().waitingPassengers--;

                    Destroy(stopParent.GetChild(0).gameObject);
                        LoadPsngrsOntoTram();

                    yield return null;
                }
                yield return null;
                _psngrIsEntering = false;
            }
        }

        public void CheckPassengerCapacity()
        {
            bool tramIsFull = SceneGlobals.currentTramData.noOfPassengers ==
                              SceneGlobals.currentTramData.maxNoOfPassengers;
            /*This checks the capacity, and decides whether to load or unload. When the bools are set, it immiediately
             sets of the loading/unloading functions in the actual substops script objects*/

            if (!stopIsOnRightSide && !stopIsOnLeftSide) return;
            var tramIsOpen = SceneGlobals.currentTramMechanics.isOpenLeftSide ||
                             SceneGlobals.currentTramMechanics.isOpenRightSide;
            //If the tram doors are closed
            if (tramIsOpen)
            {
                if (!tramIsFull)
                {
                    unloadingPassengers = false;
                    loadingPassengers = true;
                    LoadPsngrsOntoTram();
                } else
                {
                    unloadingPassengers = true;
                    loadingPassengers = false;
                    UnloadPsngrsFromTram();
                }
            }
            else
            {
                unloadingPassengers = false;
                loadingPassengers = false;
            }
        }

    }
}
