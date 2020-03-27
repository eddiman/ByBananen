using System;
using ControllerScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;

namespace InputControllerScripts
{
    public class InputSystemScript : MonoBehaviour
    {
        public GameObject tramController;
        private Vector2 _mMove;
        public void OnMove(InputAction.CallbackContext context)
        {
            _mMove = context.ReadValue<Vector2>();

        }

        public void OnOpenLeftSide(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OpenCurrentTramSideDoors("left");
            }
        }
        public void OnOpenRightSide(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OpenCurrentTramSideDoors("right");
            }
        }

        public void Update()
        {
            // Update orientation first, then move. Otherwise move orientation will lag
            // behind by one frame.
            Move(_mMove);
        }

        private void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.01)
                return;

            if (direction == Vector2.up)
            {
                Debug.Log("moving up");
                tramController.GetComponent<TramController>().GetTramMechanics().Accelerate();
            }
            if (direction == Vector2.down)
            {
                Debug.Log("moving down");
                tramController.GetComponent<TramController>().GetTramMechanics().Decelerate();
            }
        }

        private void OpenCurrentTramSideDoors(string side)
        {

            tramController.GetComponent<TramController>().GetTramMechanics().OpenSideDoors(side);
            tramController.GetComponent<TramController>().GetTramTransporting().CheckPassengerCapacity();
        }

        public void SetCurrentTramFromRayCast(InputAction.CallbackContext context)
        {
            if (context.started) {
                RaycastHit hit;
                Debug.Log("clicking");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out hit)) return;
                Debug.Log(hit.collider.gameObject.name);

                if (hit.collider.CompareTag("Tram"))
                {
                    tramController.GetComponent<TramController>().SetCurrentTram(hit.collider.gameObject);
                }
            }
        }
    }
}
