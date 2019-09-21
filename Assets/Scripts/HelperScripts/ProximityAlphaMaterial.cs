using UnityEngine;

namespace HelperScripts
{
    public class ProximityAlphaMaterial : MonoBehaviour
    {
        private Material _material;

        private void Start()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            var distance = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
            _material.SetFloat("_Distance", distance);
        }
    }
}
