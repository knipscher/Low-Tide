using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegManager : MonoBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private Leg[] legs;
    [SerializeField] private float stepSize = 1f;

    private bool isStuck = false;

    public Action onReleased;

    private void Start()
    {
        legs = GetComponentsInChildren<Leg>();

        foreach (var leg in legs)
        {
            leg.Initialize(transform.position, body);
            leg.onReleased += OnReleased;
        }
    }

    private void FixedUpdate()
    {
        if (!isStuck)
        {
            Walk();
        }
        else
        {
            SyncLegs();
        }
    }

    private void Walk()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].transform.position = body.transform.position + (body.transform.forward * legs[i].offset.z) +
                                         (body.transform.up * legs[i].offset.y) +
                                         (body.transform.right * legs[i].offset.x);
            legs[i].Walk(stepSize);
        }
    }

    private void SyncLegs()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].transform.position = body.transform.position + (body.transform.forward * legs[i].offset.z) +
                                         (body.transform.up * legs[i].offset.y) +
                                         (body.transform.right * legs[i].offset.x);
        }
    }

    public bool Capture(Animal itemToCapture)
    {
        var nearestLegID = GetClosestFootID(itemToCapture.transform.position);
        if (nearestLegID != -1)
        {
            legs[nearestLegID].AttemptCapture(itemToCapture);
            return true;
        }

        return false;
    }

    private int GetClosestFootID(Vector3 position)
    {
        int closestNodeID = -1;
        var nearestSqrDistance = Mathf.Infinity;
        for (int i = 0; i < legs.Length; i++)
        {
            if (legs[i].capturedItem == null)
            {
                var sqrDistance = (position - legs[i].foot.transform.position).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    closestNodeID = i;
                }
            }
        }

        return closestNodeID;
    }

    public int GetFreeLegCount()
    {
        var freeLegs = 0;
        foreach (var leg in legs)
        {
            if (leg.capturedItem == null)
            {
                freeLegs++;
            }
        }
        return freeLegs;
    }

    public void OnReleased()
    {
        onReleased?.Invoke();
    }
}