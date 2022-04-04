using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegNode : MonoBehaviour
{
    public void LerpToPosition(Vector3 position, float lerpSpeed)
    {
        transform.position = Vector3.Lerp(transform.position, position, lerpSpeed);
    }
}