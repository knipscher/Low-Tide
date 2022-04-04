using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public bool isPlugged { get; private set; } = false;
    [SerializeField] private Transform plugMarker;
    [SerializeField] private GameObject visuals;

    private void OnCollisionEnter(Collision collision)
    {
        Plug(collision.transform);
    }

    private void FixedUpdate()
    {
        var visualsPosition = visuals.transform.position;
        visuals.transform.position = new Vector3(visualsPosition.x, GameManager.instance.currentWaterHeight, visualsPosition.z);
        plugMarker.position = new Vector3(plugMarker.position.x, GameManager.instance.currentWaterHeight, plugMarker.position.z);
    }

    private void Plug(Transform plugTransform)
    {
        if (!isPlugged)
        {
            var animal = plugTransform.parent.GetComponent<Animal>();
            if (animal && animal.captured)
            {
                visuals.SetActive(false);
                isPlugged = true;
                animal.PlugHole(this, plugMarker);
            }
        }
    }

    public void Unplug()
    {
        isPlugged = false;
        visuals.SetActive(true);
    }
}