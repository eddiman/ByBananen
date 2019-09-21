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
        public int stopCounter;

        private float timer;
        private Vector3 thisColliderSize;
        private bool isCurrentTramStop;
        private TramData currentTramData;

        private bool passengerMoving;

        public List<Transform> arrayOfPassengers;
    
        // Start is called before the first frame update

        void Start()
        {
            timer = instantiationTimer;
            //maxPassengers = Random.Range(2, 5);

            thisColliderSize = GetComponent<Collider>().bounds.size;
            
            arrayOfPassengers = new List<Transform>();
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

                    LoadPassengers();
                
                } else if (!SceneGlobals.currentTramTransporting.loadingPassengers 
                           && SceneGlobals.currentTramTransporting.unloadingPassengers) 
                {
                    Debug.Log("unlaoding pass");
                    UnloadPassengers();

                }
            }

            if (waitingPassengers < maxPassengers)
            {
                GeneratePassenger();
            }
        }

        private void LoadPassengers()
        {
            var isLeft = false;
            isLeft = SceneGlobals.currentTramTransporting.stopIsOnLeftSide;

            if (SceneGlobals.currentTramData.currentStop == transform)
            {
                passengerMoving = true;

                if (waitingPassengers > 0 && passengerMoving)
                {

                    SceneGlobals.currentTramTransporting.isFull = false;

                    Vector3 tramDoor = SceneGlobals.currentTram.transform.Find("tram_door_" + (isLeft ? "left" : "right") +"_2").position;
                    Debug.Log(SceneGlobals.currentTram.transform.Find("tram_door_" + (isLeft ? "left" : "right") +"_2"));
                    //Vector3 tramDoorPos = new Vector3(tramDoor.x - 1, tramDoor.y - 0.5f, tramDoor.z);
                    Vector3 tramDoorPos = new Vector3(isLeft ? tramDoor.x - 1 : tramDoor.x + 1, tramDoor.y + 0.5f, tramDoor.z);

                    if (t < passengerMoveTime)
                    {
                        t += Time.deltaTime;
                        // Debug.Log(t);

                        arrayOfPassengers[stopCounter].transform.position = Vector3.Slerp(
                            arrayOfPassengers[stopCounter].transform.position,
                            tramDoorPos, t / passengerMoveTime / 10);
                        //Debug.Log(stopCounter);
                    }
                    else
                    {
                        //Debug.Log("finished");
                        arrayOfPassengers[stopCounter].transform.parent = SceneGlobals.currentTram.transform;
                        SceneGlobals.currentTramTransporting.listOfPassengers.Add(arrayOfPassengers[stopCounter]);
                        stopCounter = stopCounter + 1;
                        SceneGlobals.currentTramData.noOfPassengers++;
                        passengerMoving = false;
                        waitingPassengers--;
                        t = 0;
                    }
                }
            }
            else
            {
                passengerMoving = false;
            }
            if (SceneGlobals.currentTramMechanics.isMoving || SceneGlobals.currentTramData.noOfPassengers == SceneGlobals.currentTramData.maxNoOfPassengers)
            {
                SceneGlobals.currentTramTransporting.loadingPassengers = false;
                SceneGlobals.currentTramTransporting.isFull = true;
            }
        }

        private void UnloadPassengers()
        {
            var isLeft = false;
            isLeft = SceneGlobals.currentTramTransporting.stopIsOnLeftSide;
            
            if (SceneGlobals.currentTramData.currentStop == transform)
            {
                //Debug.Log("stop knows right side is open");
                SceneGlobals.currentTramTransporting.isEmpty = false;
                SceneGlobals.currentTramTransporting.isFull = false;
                passengerMoving = true;

                if (passengerMoving)
                {
                    Vector3 tramDoor = SceneGlobals.currentTram.transform.Find("tram_door_" + (isLeft ? "left" : "right") +"_1").position;
                    Vector3 tramDoorPos = new Vector3(isLeft ? tramDoor.x - 1 : tramDoor.x + 1, tramDoor.y + 0.5f, tramDoor.z);
                    Vector3 tramDoorPosExit = new Vector3(isLeft ? tramDoor.x - 30 : tramDoor.x + 30, tramDoor.y + 0.5f, tramDoor.z);
                    var tramCounter =  SceneGlobals.currentTramTransporting.tramPassengerCounter;
                    if (t < passengerMoveTime)
                    {
                        t += Time.deltaTime;
                        // Debug.Log(t);

                        SceneGlobals.currentTramTransporting.listOfPassengers[tramCounter].transform.position = Vector3.Slerp(
                            tramDoorPos ,tramDoorPosExit, t / passengerMoveTime / 5);

                        //Debug.Log(stopCounter);
                    }
                    else
                    {
                        //Debug.Log("finished");
                        SceneGlobals.currentTramTransporting.tramPassengerCounter = tramCounter + 1;
                        SceneGlobals.currentTramData.noOfPassengers--;
                        Destroy(SceneGlobals.currentTramTransporting.listOfPassengers[tramCounter].gameObject);
                        passengerMoving = false;
                        t = 0;
                    }
                }
            }
            else
            {
                passengerMoving = false;
            }
            if (SceneGlobals.currentTramMechanics.isMoving || SceneGlobals.currentTramData.noOfPassengers == 0)
            {
                SceneGlobals.currentTramTransporting.listOfPassengers.Clear();
                SceneGlobals.currentTramTransporting.tramPassengerCounter = 0;
                SceneGlobals.currentTramTransporting.unloadingPassengers = false;
                SceneGlobals.currentTramTransporting.isEmpty = true;

            }
        }

        private void GeneratePassenger()
        {
            var thisTransform = transform;
            timer -= Time.deltaTime;
            if (!(timer <= 0)) return;
            Object passengerPrefab = Resources.Load("Prefabs/Passenger");
            var randomParentPos = new Vector3(Random.Range(0, thisColliderSize.x), 1, Random.Range(0, thisColliderSize.z * -1));
            GameObject psngr = Instantiate(passengerPrefab) as GameObject;
            if (psngr != null)
            {
                psngr.transform.parent = thisTransform;
                psngr.transform.localPosition = randomParentPos;
                arrayOfPassengers.Add(psngr.transform);
            }

            waitingPassengers++;
            timer = instantiationTimer;
        }
    }
}