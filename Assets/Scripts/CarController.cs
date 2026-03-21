using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Components:")]
    private Rigidbody rb;
    [SerializeField] private Transform[] wheels;
    [SerializeField] private Transform centerOfMass;

    [Header("Motor")]
    public float motorStrenght;

    public float topSpeed;

    [Header("Car Steering:")]
    [SerializeField] private float turnRadius;
    [SerializeField] private float rearTrack;
    [SerializeField] private float wheelbase;

    [Header("Wheel Settings")]
    [SerializeField] private float gripStrength;
    [SerializeField] private float wheelRadius;

    public AnimationCurve torqueCurve; // Add this variable to hold the torque curve

    private float verticalInput;
    private float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        foreach (Transform wheel in wheels)
        {
            WheelScript wheelScript = wheel.GetComponent<WheelScript>();
            if (wheelScript.isMotorized)
            {
                float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
                float speedRatio = topSpeed != 0f ? forwardSpeed / topSpeed : 0f;
                float motorSpeedMultiplier = torqueCurve.Evaluate(speedRatio);

                wheelScript.motorInput = verticalInput * motorStrenght * motorSpeedMultiplier;


            }
            if (wheelScript.steers)
            {
                float steerAngle = horizontalInput * turnRadius;
                Quaternion targetRotation = Quaternion.Euler(wheel.localRotation.x, steerAngle, wheel.localRotation.z);
                wheel.localRotation = Quaternion.RotateTowards(wheel.localRotation, targetRotation, 100f * Time.deltaTime);
            }
        }
    }
}
