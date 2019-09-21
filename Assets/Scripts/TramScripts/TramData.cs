using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;

namespace TramScripts
{
    public class TramData : MonoBehaviour
    {
        public int tramNumber = 0;
        public int tramLine = 0;
        public string tramLineName;
        public int noOfPassengers = 0;
        public int maxNoOfPassengers = 0;
        public string currentPlace;
        public float distance;
        
        private Transform _tramMenu;
        
        public Transform currentStop;
        public Transform previousStop;

        
        private Camera _mainCamera;

        private TextMeshPro _tramNameTxt;
        private TextMeshPro _tramLineTxt;
        private TextMeshPro _tramPassTxt;
        private TextMeshPro _tramLocTxt;


        // Start is called before the first frame update
        void Start()
        {
            currentStop = null;
            _tramMenu = transform.Find("TramMenu");
            _tramNameTxt = _tramMenu.Find("TramNameTxt").GetComponent<TextMeshPro>();
            _tramLineTxt = _tramMenu.Find("TramLineTxt").GetComponent<TextMeshPro>();
            _tramPassTxt = _tramMenu.Find("TramPassengerTxt").GetComponent<TextMeshPro>();
            _tramLocTxt = _tramMenu.Find("TramLocationTxt").GetComponent<TextMeshPro>();
        
            _tramNameTxt.SetText("Tram " + tramNumber);
            _tramLineTxt.SetText("Line " + tramLine);
            _tramPassTxt.SetText("Psngrs. " + noOfPassengers);

            //_allLineStops = GetAllLineStopsFromLine(tramLine);
        }

        private void FixedUpdate()
        {

        }

        
        public void SetCurrentStop(Transform stop)
        {
            Debug.Log("settingh current stop");
            if (stop != null && stop != previousStop)
            {
                previousStop = stop;
            }
            if (stop == null)
            {
                currentStop = null;
                _tramLocTxt.SetText("");
            } else
            {
                //Remember, the stop is sub stop the if we want the line stop, we need the get the parent
                currentStop = stop;
                _tramLocTxt.SetText("Currently at: " + currentStop.parent.name);
            }
                
            

        }


    }
}