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

        private Vector3 thisColliderSize;
        public int subStopPsngrCounter;
        private int _noOfPassengersToLoad;

        void Start()
        {
            thisColliderSize = GetComponent<Collider>().bounds.size;

            InvokeRepeating(nameof(GenerateSubStopPassengers), 1.0f, instantiationTimer);
            for (int i=0; i < waitingPassengers; i++)
            {
                GenerateSubStopPassengers(false);
            }
        }
        private void GenerateSubStopPassengers()
        {
            if (waitingPassengers >= maxPassengers) return;
            Debug.Log("generate pass");

            var thisTransform = transform;
            // timer -= Time.deltaTime;
            //if (!(timer <= 0)) return;
            var randomParentPos = new Vector3(Random.Range(0.00180f, -0.01863f), Random.Range(-0.06732f, 0.2238f) , -0.01054f);
            GameObject psngr = PassengerHelper.CreatePassenger();
            if (psngr != null)
            {
                psngr.transform.parent = thisTransform;
                psngr.transform.localPosition = randomParentPos;
                waitingPassengers++;
            }
        }
        private void GenerateSubStopPassengers(bool addPsngrCount)
        {
            if (waitingPassengers >= maxPassengers) return;
            Debug.Log("generate pass plus plus");

            var thisTransform = transform;
            var randomParentPos = new Vector3(Random.Range(0.00180f, -0.01863f), Random.Range(-0.06732f, 0.2238f) , -0.01054f);
            GameObject psngr = PassengerHelper.CreatePassenger();
            if (psngr != null)
            {
                psngr.transform.parent = thisTransform;
                psngr.transform.localPosition = randomParentPos;
                if(addPsngrCount)
                    waitingPassengers++;
            }
        }
    }
}
