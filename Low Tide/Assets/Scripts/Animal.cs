using System.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Animal : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform models;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float escapeSpeed;

    private Vector3 currentMoveForce;

    [SerializeField] private float directionChangeTime = 1f;

    public bool captured { get; private set; } = false;
    public bool stuck { get; private set; } = false;

    [SerializeField] private float pullStrength = 1f;

    private Leg captureLeg = null;
    private Hole stuckHole = null;

    private Vector3 previousPosition;

    [SerializeField] private float minStuckTime = 10f;

    [SerializeField] private Transform ripple;

    [SerializeField] private float animalHeight;

    private void Start()
    {
        StartCoroutine(ChangeDirections());
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.hasGameStarted || GameManager.instance.isGameOver)
        {
            return;
        }

        if (stuck)
        {
            Move(0);
            return;
        }

        if (captured)
        {
            Move(escapeSpeed);
            PullTowardPoint(captureLeg.transform.position);
        }
        else
        {
            Move(moveSpeed);
        }
    }

    private void Move(float speed)
    {
        rb.AddForce(currentMoveForce * speed);
        models.transform.position = Vector3.Lerp(models.transform.position, rb.transform.position, 0.2f);
        if (rb.velocity.sqrMagnitude > 0)
        {
            models.transform.rotation = Quaternion.Slerp(models.rotation, Quaternion.LookRotation(rb.velocity.normalized, Vector3.up), 0.2f);
        }

        if (GameManager.instance.currentWaterHeight < animalHeight)
        {
            ripple.gameObject.SetActive(true);
            ripple.position = new Vector3(rb.transform.position.x, GameManager.instance.currentWaterHeight, rb.transform.position.z);
        }
        else
        {
            ripple.gameObject.SetActive(false);
        }
    }

    private IEnumerator ChangeDirections()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(directionChangeTime, directionChangeTime * 2));

            var random = Random.value;
            if (random < 0.3f || captured)
            {
                currentMoveForce = new Vector3(Random.Range(-moveSpeed, moveSpeed), 0, Random.Range(-moveSpeed, moveSpeed));
            }
            else
            {
                currentMoveForce = (Vector3.zero - rb.transform.position).normalized;
                currentMoveForce = new Vector3(currentMoveForce.x, 0, currentMoveForce.z);
            }
        }
    }

    public Vector3 GetRigidbodyCenter()
    {
        return rb.transform.position;
    }

    private void PullTowardPoint(Vector3 point)
    {
        rb.MovePosition(Vector3.Lerp(rb.transform.position, point, Time.deltaTime * pullStrength));
    }

    public void Capture(Transform body, Leg leg)
    {
        captureLeg = leg;
        captured = true;
    }

    public void Release()
    {
        if (captureLeg)
        {
            captureLeg.Release();
        }
        captureLeg = null;
        captured = false;
    }

    public void PlugHole(Hole hole, Transform plugMarker)
    {
        Release();
        stuck = true;
        previousPosition = rb.transform.position;
        rb.isKinematic = true;
        rb.position = plugMarker.position;
        rb.rotation = plugMarker.rotation;
        stuckHole = hole;
        StartCoroutine(DelayThenEscape());
    }

    private IEnumerator DelayThenEscape()
    {
        yield return new WaitForSeconds(Random.Range(minStuckTime, minStuckTime * 1.1f));
        EscapeHole();
    }

    private void EscapeHole()
    {
        rb.isKinematic = false;
        rb.transform.position = previousPosition;
        stuck = false;
        if (stuckHole)
        {
            stuckHole.Unplug();
        }

        stuckHole = null;
    }
}
