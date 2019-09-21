using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ControllerScripts
{
    /*
     * This Controller is connected to MainController-gameobject
     */
    public class MainUiController : MonoBehaviour
    {
        Transform _previousTransform;
        private GameObject[] buildingFullMeshArray;
        private GameObject[] buildingReducedMeshArray;
        void Start()
        {
//            buildingFullMeshArray = GameObject.FindGameObjectsWithTag ("BuildingFullMesh");
  //          buildingReducedMeshArray = GameObject.FindGameObjectsWithTag ("BuildingReducedMesh");
        }

        public void GoToMenu()
        {

            //Init the cam
            if(!SceneGlobals.isInMapOverview){
                _previousTransform = SceneGlobals.currentFocusedTransform;
                var transform1 = transform;
                SceneGlobals.currentFocusedTransform = transform1;
                SceneGlobals.currentFocusedTag = transform1.tag;
                foreach(GameObject go in buildingFullMeshArray)
                {
                    go.SetActive(false);
                }
                foreach(GameObject go in buildingReducedMeshArray)
                {
                    go.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else if(SceneGlobals.isInMapOverview && !SceneGlobals.camFocusOnTram)
            {
                SceneGlobals.currentFocusedTransform = _previousTransform;
                SceneGlobals.currentFocusedTag = _previousTransform.tag;
                Debug.Log(buildingReducedMeshArray.Length);

                foreach(GameObject go in buildingFullMeshArray)
                {
                    go.SetActive(true);
                }
                foreach(GameObject go in buildingReducedMeshArray)
                {
                    go.transform.GetChild(0).gameObject.SetActive(false);
                }


            }
        }
    }
}
