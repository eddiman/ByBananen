using TramScripts;
using UnityEngine;

namespace ControllerScripts
{
    public class SceneGlobals : MonoBehaviour
    {
        public static GameObject currentTram;
        public static GameObject previousTram;
        public static Transform currentFocusedTransform;
        public static string currentFocusedTag;

        public static bool isInMapOverview;

        public static bool camFocusOnTram;

        //Tramscripts
        public static TramMechanics currentTramMechanics;
        public static TramData currentTramData;
        public static TramTransporting currentTramTransporting;
        public static TramEngine currentTramEngine;

        //Junction switch vars
        public static bool canSwitchTrack;
    }
}
