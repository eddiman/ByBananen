using System.Collections.Generic;
using TramScripts;
using UnityEngine;


namespace ControllerScripts
{
    public class MainLineController : MonoBehaviour
    {
        private TramMechanics _thisTramMechanics;
        private TramTransporting _thisTramTransporting;
        private TramData _thisTramData;
        private GameObject _currentObjTram;
        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public List<Transform> GetAllLineStopsFromLine(int lineNo)
        {
            var currentTramLine = GameObject.Find("Line " + lineNo);        
            var lineStopsObj = currentTramLine.transform.Find("Line Stops");

            var tramLineStops = new List<Transform>();
            foreach (Transform stop in lineStopsObj)
            {
                foreach (var subStop in stop)
                {
                    //Debug.Log(subStop);
                }

                //There is always two substops on each stops, no exception 
                tramLineStops.Add(stop.Find("SubStop-1"));
                tramLineStops.Add(stop.Find("SubStop-2"));
            } 

            return tramLineStops;
        }
    
    }
}