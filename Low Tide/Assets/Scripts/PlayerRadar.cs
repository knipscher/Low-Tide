using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRadar : MonoBehaviour
{
    public List<Animal> animalsInRange = new List<Animal>();

    private void OnTriggerEnter(Collider other)
    {
        var animal = other.transform.parent.GetComponent<Animal>();
        if (animal)
        {
            animalsInRange.Add(animal);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var animal = other.transform.parent.GetComponent<Animal>();
        if (animal)
        {
            animalsInRange.Remove(animal);
            animal.Release();
        }
    }
}