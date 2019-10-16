using System;
using ControllerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;

namespace InputControllerScripts
{
    public class InputSystemScript : MonoBehaviour
    {

        public void OnMove(InputAction.CallbackContext context)
        {

                Debug.Log(context.ReadValue<Vector2>());

                if (context.ReadValue<Vector2>() == Vector2.up)
                {
                    Debug.Log("moving up");
                    SceneGlobals.currentTramMechanics.Accelerate();

                }
                if (context.ReadValue<Vector2>() == Vector2.down)
                {
                    Debug.Log("moving down");
                    SceneGlobals.currentTramMechanics.Decelerate();
                }



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

        private void OpenCurrentTramSideDoors(string side)
        {
            SceneGlobals.currentTramMechanics.OpenSideDoors(side);
            SceneGlobals.currentTramTransporting.CheckPassengerCapacity();
        }
    }
}
