using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;

namespace TramScripts
{
    public class TramData : MonoBehaviour
    {
        public int tramNumber;
        public int tramLine;
        public string tramLineName;
        public int noOfPassengers;
        public int maxNoOfPassengers;
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
        }

    }


}
