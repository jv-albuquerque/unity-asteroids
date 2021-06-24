using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInfo : MonoBehaviour
{
    [SerializeField] private Renderer render;

    [SerializeField] private float maxVelocity = 8;
    [SerializeField] private float moveForce = 10;
    [SerializeField] private float rotationForce = 150;

    [SerializeField] private float recoil = 3;
    [SerializeField] private GameObject bulletObject;
    [SerializeField] private List<Transform> shootPos = new List<Transform>();

    [SerializeField] private AudioClip shootAudio;
    [SerializeField] private AudioClip motorAudio;

    public Renderer Renderer { get => render; }

    public float MaxVelocity { get => maxVelocity; }
    public float MoveForce { get => moveForce; }
    public float RotationForce { get => rotationForce; }

    public float Recoil { get => recoil; }
    public GameObject Bullet { get => bulletObject; }
    public List<Transform> ShootPos { get => shootPos; }

    public AudioClip ShootAudio { get => shootAudio; }
    public AudioClip MotorAudio { get => motorAudio; }
}
