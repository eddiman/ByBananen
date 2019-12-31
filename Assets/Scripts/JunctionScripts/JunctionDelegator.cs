using System;
using System.Net;
using ControllerScripts;
using TramScripts;
using UnityEngine;

namespace JunctionScripts
{
    //The junction checks whether the player is
    public class JunctionDelegator : MonoBehaviour
    {
        public float closestDistance = 10f;


        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.GetComponent<TramEngine>()) return;
            other.gameObject.GetComponent<TramEngine>().canSwitchTracks = true;

        }

       /* private void Update()
        {
            if (SceneGlobals.currentTram == null) return;

            Debug.DrawRay(transform.position, transform.forward * closestDistance, Color.red);
            Debug.DrawRay(transform.position, (transform.forward * -1) * closestDistance, Color.red);
            float distance = Vector3.Distance(SceneGlobals.currentTram.transform.position, transform.position);
            if (distance < closestDistance)
            {
                SceneGlobals.canSwitchTrack = true;
                Debug.Log("in range");

            }
            else
            {
                SceneGlobals.canSwitchTrack = false;

            }

        }*/
    }
}
