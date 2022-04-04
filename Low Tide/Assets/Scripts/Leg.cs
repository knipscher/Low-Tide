using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Leg : MonoBehaviour
{
    [HideInInspector] public Vector3 footPosition;
    [HideInInspector] public Vector3 offset;
    private Transform body;

    [SerializeField] private Vector3 stepOffset;

    [SerializeField] private LegNode[] legNodes;

    [SerializeField] private float stepLerpSpeed = 0.3f;

    [SerializeField] private float randomOffsetMultiplier = 0.3f;

    [HideInInspector] public LegNode foot;

    public Animal capturedItem;

    public Action onReleased;

    public void Initialize(Vector3 legManagerPosition, Transform body)
    {
        offset = transform.position - legManagerPosition;
        this.body = body;
        Walk(1);
        foot = legNodes[legNodes.Length - 1];
    }

    public void Walk(float stepDistance)
    {
        if (capturedItem != null)
        {
            MoveFootToCapturedItem();
        }
        else
        {
            if (IsFootDistanceOverThreshold(stepDistance))
            {
                MoveFootPositionForward();
            }
            MoveFootPositionToFloor();
        }

        LerpLegNodes();
    }

    private bool IsFootDistanceOverThreshold(float stepDistance)
    {
        return Vector3.Distance(footPosition, transform.position) > stepDistance;
    }

    private void MoveFootPositionForward()
    {
        footPosition = transform.position + (body.transform.right * stepOffset.x);
        var randomOffset = new Vector3(Random.Range(-stepOffset.x, stepOffset.x), 0, Random.Range(-stepOffset.z, stepOffset.z)) * randomOffsetMultiplier;
        footPosition += randomOffset;
    }

    private void MoveFootPositionToFloor()
    {
        footPosition = new Vector3(footPosition.x, 0, footPosition.z);
    }

    private void LerpLegNodes()
    {
        var lastNodeID = legNodes.Length - 1;

        var lerpSpeed = capturedItem == null ? stepLerpSpeed : stepLerpSpeed * 2f;

        for (int i = 0; i < legNodes.Length; i++)
        {
            if (i < lastNodeID)
            {
                var lerpRatio = i / (float)lastNodeID;
                var position = Vector3.Lerp(transform.position, footPosition, lerpRatio);
                legNodes[i].LerpToPosition(position, lerpSpeed * lerpRatio);
                legNodes[i].transform.rotation = Quaternion.LookRotation(legNodes[i + 1].transform.position - legNodes[i].transform.position) * Quaternion.Inverse((Quaternion.Euler(89.98f, 180, 0)));
            }
            else if (i == lastNodeID)
            {
                legNodes[i].LerpToPosition(footPosition, lerpSpeed);
                legNodes[i].transform.rotation = legNodes[i - 1].transform.rotation;
            }
        }
    }

    public void AttemptCapture(Animal itemToCapture)
    {
        if (!itemToCapture.stuck)
        {
            capturedItem = itemToCapture;
            capturedItem.Capture(transform, this);
        }
    }

    public void Release()
    {
        capturedItem = null;
        onReleased?.Invoke();
    }

    private void MoveFootToCapturedItem()
    {
        footPosition = capturedItem.GetRigidbodyCenter();
    }
}