using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidImage : MonoBehaviour
{
    private float clockwise;
    private float rotationForce;

    // Start is called before the first frame update
    void Start()
    {
        clockwise = Random.Range(0, 2);
        clockwise = clockwise * 2 - 1;

        rotationForce = Random.Range(50, 100);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(transform.position, Vector3.forward, clockwise * rotationForce * Time.fixedDeltaTime);
    }
}
