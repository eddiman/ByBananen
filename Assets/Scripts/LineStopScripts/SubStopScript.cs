using System;
using System.Collections;
using System.Collections.Generic;
using ControllerScripts;
using TramScripts;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LineStopScripts
{
    /***
     * This entire class is wacky af
     * @author Edvard Pires Bjørgen, 2019
     */
    public class SubStopScript : MonoBehaviour
    {
        public int waitingPassengers;
        public int maxPassengers = 5;
        public float instantiationTimer = 1f;
        private float passengerMoveTime = 1f;
        private float t;
        private float t2;
        public int stopCounter;

        private float timer;
        private Vector3 thisColliderSize;
        private bool isCurrentTramStop;
        private TramData currentTramData;

        private bool passengerMoving;
        private GameObject[] _allStops;
        public List<Transform> arrayOfPassengers;

        private int _noOfPassengersToLoad;
        // Start is called before the first frame update

        void Start()
        {
            timer = instantiationTimer;
            //maxPassengers = Random.Range(2, 5);

            thisColliderSize = GetComponent<Collider>().bounds.size;

            arrayOfPassengers = new List<Transform>();
            _allStops = GameObject.FindGameObjectsWithTag("TramStop");
        }

        void Update()
        {


            if (SceneGlobals.currentTram != null)
            {


                //Check if the trams current stop is

                if (SceneGlobals.currentTramTransporting.loadingPassengers
                    && !SceneGlobals.currentTramTransporting.unloadingPassengers)
                {
                    Debug.Log("laoding pass");

                    LoadPassengersIntoTram();

                }
                else if (!SceneGlobals.currentTramTransporting.loadingPassengers
                         && SceneGlobals.currentTramTransporting.unloadingPassengers)
                {
                    Debug.Log("unlaoding pass");
                    UnloadPassengersFromTram();

                }
            }

            if (waitingPassengers < maxPassengers)
            {
                GeneratePassenger();
            }
        }

        private void LoadPassengersIntoTram()
        {
            var isLeft = false;
            isLeft = SceneGlobals.currentTramTransporting.stopIsOnLeftSide;

            if (SceneGlobals.currentTramData.currentStop == transform)
            {
                passengerMoving = true;

                if (waitingPassengers > 0 && passengerMoving)
                {

                    SceneGlobals.currentTramTransporting.isFull = false;

                    //Vector3 tramDoor = SceneGlobals.currentTram.transform.Find("tram_door_" + (isLeft ? "left" : "right") +"_2").position;
                    Vector3 tramDoor = SceneGlobals.currentTram.transform.position;
                    //Debug.Log(SceneGlobals.currentTram.transform.Find("tram_door_" + (isLeft ? "left" : "right") +"_2"));
                    //Vector3 tramDoorPos = new Vector3(tramDoor.x - 1, tramDoor.y - 0.5f, tramDoor.z);
                    Vector3 tramDoorPos = new Vector3(isLeft ? tramDoor.x - 1 : tramDoor.x + 2, tramDoor.y-.5f,
                        tramDoor.z + 3);
                    Vector3 tramOutsideDoorPos = new Vector3(isLeft ? tramDoor.x - 1 : tramDoor.x + 2, tramDoor.y- .5f,
                        tramDoor.z - 2);
                    /*OK, gotta write this shit down or else imma forget the logic later:
                    the var t is the first step, this one is for when the passenger goes to the outer front of the tram
                    door, the "else" in t is when it is finished and on position,
                    t2 is the second step, it slerps in the tram, and then on the "else"-block, this script  registers
                    the passenger in the tram scripts*/
                    if (t < passengerMoveTime)
                    {
                        t += Time.deltaTime;
                        // Debug.Log(t);

                        arrayOfPassengers[stopCounter].transform.position = Vector3.Slerp(
                            arrayOfPassengers[stopCounter].transform.position,
                            tramOutsideDoorPos, t / passengerMoveTime / 10);
                        //Debug.Log(stopCounter);


                    } else
                    {
                        if (t2 < passengerMoveTime)
                        {
                            t2 += Time.deltaTime;
                            // Debug.Log(t);

                            arrayOfPassengers[stopCounter].transform.position = Vector3.Slerp(
                                arrayOfPassengers[stopCounter].transform.position,
                                tramDoorPos, t2 / passengerMoveTime / 50);
                            //Debug.Log(stopCounter);
                        } else
                        {
                            //Debug.Log("finished");
                            arrayOfPassengers[stopCounter].transform.parent = SceneGlobals.currentTram.transform;
                            SceneGlobals.currentTramTransporting.listOfPassengers.Add(arrayOfPassengers[stopCounter]);
                            stopCounter = stopCounter + 1;
                            SceneGlobals.currentTramData.noOfPassengers++;
                            passengerMoving = false;
                            waitingPassengers--;
                            t = 0;
                            t2 = 0;
                        }
                    }
                }
            }
            else
            {
                passengerMoving = false;
            }

            if (SceneGlobals.currentTramMechanics.isMoving || SceneGlobals.currentTramData.noOfPassengers ==
                SceneGlobals.currentTramData.maxNoOfPassengers)
            {
                SceneGlobals.currentTramTransporting.loadingPassengers = false;
                SceneGlobals.currentTramTransporting.isFull = true;
            }
        }

        private void UnloadPassengersFromTram()
        {
            var isLeft = false;
            //Used to determine if the stop is on left side of tram, if false, its on right side
            isLeft = SceneGlobals.currentTramTransporting.stopIsOnLeftSide;
            var tramCounter = SceneGlobals.currentTramTransporting.tramPassengerCounter;
            var currentPassenger = SceneGlobals.currentTramTransporting.listOfPassengers[tramCounter];
            var currentStop = SceneGlobals.currentTramData.currentStop;

            if (currentStop == transform)
            {

                //Debug.Log("stop knows right side is open");
                //At this point tram isnt full, or the function wouldnt run
                SceneGlobals.currentTramTransporting.isEmpty = false;
                SceneGlobals.currentTramTransporting.isFull = false;



                passengerMoving = true;
                //Check that passenger is not loaded from currentstop, and unload it if currenStop corresponds with its
                //endstop
                if (currentPassenger.GetComponent<PassengerScript>().startStop != currentStop &&
                    currentPassenger.GetComponent<PassengerScript>().endStop == currentStop )
                {
                    if (passengerMoving)
                    {
                        //Vector3 tramDoor = SceneGlobals.currentTram.transform.Find("tram_door_" + (isLeft ? "left" : "right") +"_1").position;
                        Vector3 tramDoor = SceneGlobals.currentTram.transform.position;
                        Vector3 tramDoorPos = new Vector3(isLeft ? tramDoor.x - 1 : tramDoor.x + 1, tramDoor.y + 0.5f,
                            tramDoor.z);
                        Vector3 tramDoorPosExit = new Vector3(isLeft ? tramDoor.x - 30 : tramDoor.x + 30,
                            tramDoor.y + 0.5f, tramDoor.z);


                        if (t < passengerMoveTime)
                        {
                            t += Time.deltaTime;
                            // Debug.Log(t);

                            currentPassenger.transform.position = Vector3.Slerp(
                                tramDoorPos, currentStop.position, t / passengerMoveTime / 5);

                            //Debug.Log(stopCounter);
                        }
                        else
                        {
                            //Debug.Log("finished");
                            SceneGlobals.currentTramTransporting.tramPassengerCounter = tramCounter + 1;
                            SceneGlobals.currentTramData.noOfPassengers--;
                            //No need to keep the passengers around, so kill them lol
                            Destroy(SceneGlobals.currentTramTransporting.listOfPassengers[tramCounter].gameObject);
                            passengerMoving = false;
                            t = 0;
                        }
                    }
                }
                else
                {
                    SceneGlobals.currentTramTransporting.tramPassengerCounter = tramCounter + 1;
                }
            }
            else
            {
                passengerMoving = false;
            }

            if (SceneGlobals.currentTramData.noOfPassengers == 0)
            {
                SceneGlobals.currentTramTransporting.listOfPassengers.Clear();
                SceneGlobals.currentTramTransporting.tramPassengerCounter = 0;
                SceneGlobals.currentTramTransporting.unloadingPassengers = false;
                SceneGlobals.currentTramTransporting.isEmpty = true;
            } else if (SceneGlobals.currentTramMechanics.isMoving)
            {
                SceneGlobals.currentTramTransporting.unloadingPassengers = false;
            }
        }

        private void GeneratePassenger()
        {
            var thisTransform = transform;
            timer -= Time.deltaTime;
            if (!(timer <= 0)) return;
            Object passengerPrefab = Resources.Load("Prefabs/Passenger");
            var randomParentPos = new Vector3(Random.Range(-0.02f, 0.02f), Random.Range(-0.03f, 0.06f), 0);
            GameObject psngr = Instantiate(passengerPrefab) as GameObject;
            if (psngr != null)
            {
                psngr.GetComponent<PassengerScript>().startStop = transform;
                psngr.GetComponent<PassengerScript>().endStop = GetRandomEndStop();
                psngr.transform.parent = thisTransform;
                psngr.transform.localPosition = randomParentPos;
                arrayOfPassengers.Add(psngr.transform);
            }

            waitingPassengers++;
            timer = instantiationTimer;
        }

        private Transform GetRandomEndStop()
        {
            //TODO: implement stops on a graph-like functionality, so that passengers cant set previous stops as end stop

            int i = Random.Range(0, _allStops.Length);
            Transform endStop = _allStops[i].transform;
            if (endStop == transform)
            {
                GetRandomEndStop();
            }
            else
            {
                return endStop;
            }

            return endStop;
        }

    }
}
