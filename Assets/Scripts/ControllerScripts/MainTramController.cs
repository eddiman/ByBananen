using System;
using InputControllerScripts;
using TramScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using static ControllerScripts.SceneGlobals;
namespace ControllerScripts
{
    public class MainTramController : MonoBehaviour
    {
        private string layerTag = "Tram";

        private Transform _closestStop;

        //ONLY FOR BYBANEN THE GAME
        private void Start()
        {
            SetRaycastTramHit(GameObject.Find("bybananen_front").transform);
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {

                    if (hit.collider.CompareTag(layerTag))
                    {
                        Debug.Log("hit");
                        SetRaycastTramHit(hit.transform);
                    } if (hit.collider.gameObject.name == "TramMenu")
                    {
                        SetRaycastTramHit(hit.collider.transform.parent);

                    }
                }
            }
        }

        private void SetRaycastTramHit(Transform tramTransform)
        {
            currentFocusedTransform = tramTransform;
            currentFocusedTag = tramTransform.tag;
            previousTram = currentTram;
            currentTram = tramTransform.gameObject;

            //Setting the script references to current tram in sceneglobals
            currentTramMechanics = currentTram.GetComponent<TramMechanics>();
            currentTramTransporting = currentTram.GetComponent<TramTransporting>();
            currentTramMechanics.UpdateFollower();
            currentTramData = currentTram.GetComponent<TramData>();
            currentTramEngine = currentTram.GetComponent<TramEngine>();

        }

        void OnGUI()
        {
            KeyboardControlTram();
        }

        public void KeyboardControlTram()
        {
            Event e = Event.current;
            if (e.isKey && e.type == EventType.KeyUp){
                switch(e.keyCode){

                    case KeyCode.Space:
                        SwitchCurrentTramDirection();
                        break;
                }
            }
        }


        public void AccelCurrentTram()
        {
            currentTramMechanics.Accelerate();
        }
        public void DecelCurrentTram()
        {
            currentTramMechanics.Decelerate();
        }

        public void OpenCurrentTramSideDoors(string side)
        {
            currentTramMechanics.OpenSideDoors(side);
            currentTramTransporting.CheckPassengerCapacity();
        }

        public void SwitchCurrentTramDirection()
        {
            currentTramMechanics.SwitchDirection();
        }

    }
}
