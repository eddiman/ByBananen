using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using ControllerScripts;
using HelperScripts;
using LineStopScripts;
using UnityEngine;
using UnityEngine.Events;
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
        public bool hasLoadedOnThisStop;
        public UnityEvent onPassengerLoadedFinished;
        public UnityEvent onPassengerUnloadedFinished;


        private bool _psngrIsEntering = true;
        private bool _psngrIsExiting = true;

        private float t = 0f;
        private float t2 = 0f;
        // Start is called before the first frame update

        private void UnloadPsngrsFromTram()
        {
            if (unloadingPassengers == false) return;

            unloadingPassengers = true;

            int unloadAmountThreshold = SceneGlobals.currentTramData.maxNoOfPassengers - SceneGlobals.currentTramData.noOfPassengers;
            int unloadAmount = Random.Range(0, SceneGlobals.currentTramData.noOfPassengers);
            _psngrIsExiting = true;
            Debug.Log("unload amount: " + unloadAmount + "|" + "treshold" + 5);
            if (_psngrIsExiting && unloadAmount > 5)
            {
                StartCoroutine(PassengerExitCoroutine());
            }
            else
            {
                unloadingPassengers = false;
                //This only runs once, even if Rider says it doesnt
                loadingPassengers = true;
                LoadPsngrsOntoTram();
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
                onPassengerUnloadedFinished.Invoke();
                SceneGlobals.currentTramData.noOfPassengers--;
                t = 0;
                UnloadPsngrsFromTram();
                yield return null;
            }
            yield return null;
            _psngrIsExiting = false;
        }
        private void LoadPsngrsOntoTram()
        {
            if (loadingPassengers == false) return;

            loadingPassengers = true;
            Transform currentStop = SceneGlobals.currentTramData.currentStop;
            SubStopScript currentStopData =  currentStop.GetComponent<SubStopScript>();
            bool tramIsFull = SceneGlobals.currentTramData.noOfPassengers ==
                              SceneGlobals.currentTramData.maxNoOfPassengers;

            //int loadAmountThreshold = currentStopData.waitingPassengers - currentStopData.waitingPassengers;
            //int loadAmount = Random.Range(0, currentStopData.waitingPassengers);
            _psngrIsEntering = true;
            if (_psngrIsEntering && !tramIsFull && currentStopData.waitingPassengers != 0)
            {
                StartCoroutine(PassengerEnterCoroutine());
            }
            else
            {
                loadingPassengers = false;
                hasLoadedOnThisStop = true;
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
            Transform tramDoor = getClosestTramDoor(stopParent.GetChild(0).gameObject);

            Vector3 psngrMoveStep1 = new Vector3(tramDoor.position.x + 1f, tramDoor.position.y - .5f,
                tramDoor.position.z - 3.5f ) ;
            Vector3 psngrMoveStep2 = tramDoor.position;


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

                    stopParent.GetChild(0).gameObject.transform.position = Vector3.Lerp(
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
                    onPassengerLoadedFinished.Invoke();
                    LoadPsngrsOntoTram();

                    yield return null;
                }
                yield return null;
                _psngrIsEntering = false;
            }
        }

        public void CheckPassengerCapacity()
        {
            bool tramIsOpen = SceneGlobals.currentTramMechanics.isOpenLeftSide ||
                              SceneGlobals.currentTramMechanics.isOpenRightSide;

            if (SceneGlobals.currentTramMechanics.isOpenLeftSide != this.stopIsOnLeftSide || SceneGlobals.currentTramMechanics.isOpenRightSide != this.stopIsOnRightSide)
                return;
            if (unloadingPassengers || loadingPassengers || SceneGlobals.currentTramData.currentStop == null)
            {
                unloadingPassengers = false;
                loadingPassengers = false;
                return;
            }

            if (hasLoadedOnThisStop) return;
            unloadingPassengers = true;
            UnloadPsngrsFromTram();
        }

        private Transform getClosestTramDoor(GameObject passenger)
        {
            GameObject[] doorList = GameObject.FindGameObjectsWithTag("TramDoorEntryPos");
            Transform bestDoor = doorList[Random.Range(0, doorList.Length)].transform;
            Debug.Log(doorList.Length);

            foreach (var door in doorList)
            {
                float distance = Vector3.Distance(door.transform.position, passenger.transform.position);
                if (distance < 10)
                {
                    bestDoor = door.transform;
                }

            }

            return bestDoor;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("TramStopDetectorCollider"))
            {
                GameObject currentStop = other.gameObject.GetComponent<StopTriggerScript>().parentStop;
                bool stopIsLeftSide = other.gameObject.GetComponent<StopTriggerScript>().stopIsLeftSide;

                //Using this qualifier for specifying that these vars are from this class, and not the trigger
                this.stopIsOnLeftSide = stopIsLeftSide;
                this.stopIsOnRightSide = !stopIsLeftSide;

                SceneGlobals.currentTramData.currentStop = currentStop.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("TramStopDetectorCollider"))
            {
                GameObject currentStop = other.gameObject.GetComponent<StopTriggerScript>().parentStop;

                this.stopIsOnLeftSide = false;
                this.stopIsOnRightSide = false;

                SceneGlobals.currentTramData.currentStop = null;
                SceneGlobals.currentTramData.previousStop = currentStop.transform;
                hasLoadedOnThisStop = false;
            }
        }
    }
}
