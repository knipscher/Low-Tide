using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform models;
    [SerializeField] private float moveSpeed;

    [SerializeField] private float horizontalSpeedRatio = 0.66f;

    [SerializeField] private Transform mainCamera;

    [SerializeField] private PlayerRadar playerRadar;
    [SerializeField] private LegManager legManager;

    [SerializeField] private Transform ripple;

    [SerializeField] private SoundEffectPlayer popSoundEffectPlayer;
    [SerializeField] private SoundEffectPlayer bloopSoundEffectPlayer;

    private void Start()
    {
        legManager.onReleased += PlayPopSound;
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isGameOver)
        {
            Move();
        }
    }

    private void Update()
    {
        if (!GameManager.instance.isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Capture();
            }
        }
    }

    private void Move()
    {
        var moveForce = mainCamera.forward * Input.GetAxis("Vertical") + mainCamera.right * Input.GetAxis("Horizontal") * horizontalSpeedRatio;
        rb.AddForce((moveForce * moveSpeed * legManager.GetFreeLegCount() / 8) + moveForce * moveSpeed / 4);
        models.transform.position = rb.transform.position;
        models.transform.rotation = Quaternion.LookRotation(rb.velocity.normalized, Vector3.up);

        ripple.position = new Vector3(rb.transform.position.x, GameManager.instance.currentWaterHeight, rb.transform.position.z);
    }

    private void Capture()
    {
        rb.velocity *= 0.1f;
        bloopSoundEffectPlayer.PlayRandomClip();
        foreach (var animal in playerRadar.animalsInRange)
        {
            if (animal.captured == false)
            {
                legManager.Capture(animal);
                popSoundEffectPlayer.PlayRandomClip();
            }
        }
    }

    private void PlayPopSound()
    {
        popSoundEffectPlayer.PlayRandomClip();
    }
}
