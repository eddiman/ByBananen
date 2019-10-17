using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionScript : MonoBehaviour
{
    public GameObject explosion; // drag your explosion prefab here
    public bool destroyBoth;

    public bool onlyExplodeOnTramCollision;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision){
        if (destroyBoth)
        {
            Explode(gameObject);
            Explode(collision.gameObject);
        } else if (onlyExplodeOnTramCollision && collision.gameObject.name.Contains("bybananen"))
        {
            Explode(gameObject);
        }
        else
        {
            //Explode(gameObject);
        }

    }
    private void Explode(GameObject gObj)
    {
        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
        Destroy(gObj); // destroy the object
        Destroy(expl, 1); // delete the explosion after 3 seconds
    }
}
