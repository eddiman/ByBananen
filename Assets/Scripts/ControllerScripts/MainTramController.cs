using System;
using InputControllerScripts;
using TramScripts;
using UnityEngine;
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
        private void FixedUpdate()
        {

            GetStopFromSides();

        }

        private void GetStopFromSides()
        {
            if (currentTram == null) return;
            var trans = currentTram.transform;

            RaycastHit hit;
            if (Physics.Raycast(currentTram.transform.position, trans.TransformDirection(Vector3.left), out hit,
                currentTramData.distance, LayerMask.GetMask("TramSubStop"))){
                Debug.DrawRay(trans.position, trans.TransformDirection(Vector3.left) * currentTramData.distance, Color.green);
                var currentStop = hit.transform;

                //Again remember, the currentstop is the substop, not the parent, line stop, wrote this cmnt because it used to be the opposite
                if (!Equals(currentStop, currentTramData.currentStop))
                {
                    //sends the substop, we need exactly where we are
                    currentTramData.SetCurrentStop(currentStop);
                }
                currentTramTransporting.stopIsOnLeftSide = true;

            } else if (Physics.Raycast(trans.position, trans.TransformDirection(Vector3.right), out hit,
                currentTramData.distance, LayerMask.GetMask("TramSubStop"))){
                Debug.DrawRay(trans.position, trans.TransformDirection(Vector3.right) * currentTramData.distance, Color.green);
                var currentStop = hit.transform;

                //Again remember, the currentstop is the substop, not the parent, line stop, wrote this cmnt because it used to be the opposite
                if (!Equals(currentStop, currentTramData.currentStop))
                {
                    //sends the substop, we need exactly where we are
                    currentTramData.SetCurrentStop(currentStop);
                }
                currentTramTransporting.stopIsOnRightSide = true;
            } else
            {
                if (!Equals(currentTramData.currentStop, null))
                {
                    currentTramData.SetCurrentStop(null);
                }
                Debug.DrawRay(trans.position, trans.TransformDirection(Vector3.left) * currentTramData.distance,
                    Color.red);
                Debug.DrawRay(trans.position, trans.TransformDirection(Vector3.right) * currentTramData.distance,
                    Color.red);
                currentTramTransporting.stopIsOnRightSide = false;

                currentTramTransporting.stopIsOnLeftSide = false;
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
            if (e.isKey && e.type == EventType.KeyDown){
                switch(e.keyCode){
                    /*MOVEMENT FUNCTIONS*/
                    case KeyCode.W:
                        AccelCurrentTram();
                        break;
                    case KeyCode.S:
                        DecelCurrentTram();
                        break;
                    /*MOVEMENT END*/

                    /*AUXILIARY FUNCTIONS*/
                    case KeyCode.R: RingCurrentTramBell(); break;
                    case KeyCode.Q: OpenCurrentTramSideDoors("left"); break;
                    case KeyCode.E: OpenCurrentTramSideDoors("right"); break;
                    /*AUXILIARY FUNCTIONS END*/

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

        public void RingCurrentTramBell()
        {
            currentTramMechanics.RingBell();
        }

        public void SwitchCurrentTramDirection()
        {
            currentTramMechanics.SwitchDirection();
        }

    }
}
