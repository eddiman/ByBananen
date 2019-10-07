using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
   // ShakeableTransform target;

    float delay = 0f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);

        GetComponent<ParticleSystem>().Play();

    }
}
