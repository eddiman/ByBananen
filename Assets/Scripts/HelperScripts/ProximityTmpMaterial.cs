using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace HelperScripts
{
    public class ProximityTmpMaterial : MonoBehaviour
    {
        private Material _material;
        [FormerlySerializedAs("Distance")] public float distance = 0;
        [FormerlySerializedAs("FadeStartDistance")] public float fadeStartDistance = 100;
        [FormerlySerializedAs("FadeCompleteDistance")] public float fadeCompleteDistance = 80;
        private TextMeshPro  _textmeshPro;
        private Color _color;

        private void Start()
        {
            _textmeshPro = GetComponent<TextMeshPro>();

        }

        private void Update()
        {
            _color = _textmeshPro.color;

            distance = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
            if (distance > fadeStartDistance) {
                return; // no change
            }
            if (distance > fadeCompleteDistance) {
                _color.a  = (distance - fadeCompleteDistance) / (fadeStartDistance - fadeCompleteDistance);
                _textmeshPro.color = _color;
            } else
            {
                _color.a = 0;
                _textmeshPro.color = _color;
            }

        }
    }
}