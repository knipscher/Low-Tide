using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour
{
    [SerializeField] private Transform targetToTrack = null;
    [SerializeField] private Rigidbody trackedRigidbody;
    [SerializeField] private float rotationLerpSpeed = 50f;
    [SerializeField] private float movementLerpSpeed = 50f;

    [SerializeField] private float offsetDistance = 2.25f;
    [SerializeField] private float yOffset = 2f;

    private Transform lookAtTransform;

    private void Start()
    {
        lookAtTransform = new GameObject("Look At Transform").transform;
        lookAtTransform.SetParent(transform);
        lookAtTransform.position = transform.position;

        trackedRigidbody = targetToTrack.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        lookAtTransform.LookAt(targetToTrack);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTransform.rotation, rotationLerpSpeed);
        if (trackedRigidbody.velocity.magnitude > 0)
        {
            transform.position = Vector3.Lerp(transform.position, targetToTrack.position - (trackedRigidbody.velocity).normalized * offsetDistance + new Vector3(0, yOffset, 0), movementLerpSpeed);
        }
    }
}