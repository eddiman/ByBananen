using System;
using System.Collections;
using System.Collections.Generic;
using ControllerScripts;
using TramScripts;
using UnityEditor;
using UnityEngine;
using System.Linq;
using HelperScripts;
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
        public int subStopPsngrCounter;
        private int _noOfPassengersToLoad;

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
//                    Debug.Log("laoding pass");

                    //LoadPassengersIntoTram();

                }
                else if (!SceneGlobals.currentTramTransporting.loadingPassengers
                         && SceneGlobals.currentTramTransporting.unloadingPassengers)
                {
                    //UnloadPassengersFromTram();

                }
            }

            if (waitingPassengers < maxPassengers)
            {
                GenerateSubStopPassengers();
            }
        }
        private void GenerateSubStopPassengers()
        {
            var thisTransform = transform;
            timer -= Time.deltaTime;
            if (!(timer <= 0)) return;
            var randomParentPos = new Vector3(Random.Range(0.00180f, -0.01863f), Random.Range(-0.06732f, 0.2238f) , -0.01054f);
            GameObject psngr = PassengerHelper.CreatePassenger();
            if (psngr != null)
            {
                psngr.transform.parent = thisTransform;
                psngr.transform.localPosition = randomParentPos;
            }

            waitingPassengers++;
            timer = instantiationTimer;
        }

    }
}
