using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TranslateFromToPoint : MonoBehaviour
{

    public GameObject objectToTranslate;
    public GameObject fromObject;
    public GameObject toObject;
    public float duration = 1f;
    public bool disableMeshRendererAtRuntime = true;
    public UnityEvent onStart;
    public UnityEvent onFinished;
    private Vector3 _originPosition;


    void Start()
    {
        toObject.GetComponent<MeshRenderer>().enabled = !disableMeshRendererAtRuntime;
        var fromPosition = fromObject.transform.position;
        objectToTranslate.transform.position = fromPosition;
       // StartCoroutine(MoveToPosition(fromObject.transform.position, toObject.transform.position, objectToTranslate, duration));
       if(fromObject != objectToTranslate)
            fromObject.GetComponent<MeshRenderer>().enabled = !disableMeshRendererAtRuntime;

    }

    public void StartTranslationToMesh()
    {
        _originPosition = fromObject.transform.localPosition;
        StartCoroutine(MoveToPosition(fromObject.transform.localPosition, toObject.transform.localPosition, objectToTranslate, duration));
    }
    public void StartTranslationToOriginFromMesh()
    {
        StartCoroutine(MoveToPosition(toObject.transform.localPosition, _originPosition, objectToTranslate, duration));
    }


    bool _isMoving = false;


   private IEnumerator MoveToPosition(Vector3 fromPosition, Vector3 toPosition, GameObject objToTranslate, float duration)
    {
        //Make sure there is only one instance of this function running
        if (_isMoving)
        {
            yield break; ///exit if this is still running
        }
        onStart.Invoke();
        _isMoving = true;

        float counter = 0;

        //Get the current position of the object to be moved

        while (counter < duration)
        {
            counter += Time.deltaTime;
            objToTranslate.transform.localPosition = Vector3.Lerp(fromPosition, toPosition, counter / duration);
            yield return null;
        }
        onFinished.Invoke();
        _isMoving = false;
    }
}
