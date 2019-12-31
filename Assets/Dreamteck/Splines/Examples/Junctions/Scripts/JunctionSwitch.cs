using UnityEngine.Serialization;

namespace Dreamteck.Splines.Examples
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class JunctionSwitch : MonoBehaviour
    {
        [System.Serializable]
        public class Bridge
        {
            public enum Direction { Forward = 1, Backward = -1, None = 0 }
            public bool active = true;
            public bool isTrackSwitcher;
            [FormerlySerializedAs("a")] public int aIndex = 0;
            //public Direction aDirection = Direction.None;
            [FormerlySerializedAs("b")] public int bIndex = 1;
            //public Direction bDirection = Direction.None;
        }


        public Bridge bridge;

        private void OnValidate()
        {
            Node node = GetComponent<Node>();
            Node.Connection[] connections = node.GetConnections();
            if (bridge == null) return;
            if (bridge.aIndex < 0) bridge.aIndex = 0;
                if (bridge.bIndex < 0) bridge.bIndex = 0;
                if (bridge.aIndex >= connections.Length) bridge.aIndex = connections.Length - 1;
                if (bridge.bIndex >= connections.Length) bridge.bIndex = connections.Length - 1;
            }
        }
}
