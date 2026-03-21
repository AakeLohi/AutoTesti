using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    public bool hasHit = false;

    [Header("Components:")]
    private Rigidbody rb;
    [SerializeField] private Transform wheelMesh;

    [Header("Suspension:")]

    public float springStrenght;
    public float dampening;

    public float restLenght;
    public float springTravel;

    private float minLength;
    private float maxLength;
    private float lastLenght;
    private float springVelocity;
    public float springDistance;

    [Header("Wheel")]

    [SerializeField] private float tireGrip;

    [SerializeField] private float wheelRadius;

    public AnimationCurve sidewaysSlipCurve;

    public float wheelsMassOfTheCar;

    public bool isMotorized = false;

    public bool steers = false;

    public float motorInput;
    public float steeringInput;

    [SerializeField] private float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, restLenght + wheelRadius))
        {
            hasHit = true;
            
            //Suspension
            Vector3 wheelVelocityWorld = rb.GetPointVelocity(hit.point);
            Vector3 wheelVelocityLocal = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

            springDistance = hit.distance - wheelRadius;

            float offset = restLenght - springDistance;

            if (offset < 0f) offset = 0;
            
            float springForce = (springStrenght * offset) - (dampening * wheelVelocityLocal.y);

            if (springForce < 0f) springForce *= 0.01f;
            
            rb.AddForceAtPosition(springForce * transform.up, hit.point);

            
            //Grip
            float steeringVel = Vector3.Dot(transform.right, wheelVelocityWorld);
            float tireSpeedPercentage = steeringVel / wheelVelocityWorld.magnitude;

            float desiredVelChange = -steeringVel * sidewaysSlipCurve.Evaluate(Mathf.Abs(tireSpeedPercentage));

            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;

            rb.AddForceAtPosition(transform.right * (desiredAccel * rb.mass * wheelsMassOfTheCar), hit.point);

            Debug.Log("Multiplier: " + sidewaysSlipCurve.Evaluate(Mathf.Abs(tireSpeedPercentage)) + "NormalizedForce: " + tireSpeedPercentage);

            //MotorTorque

            float accelVel = Vector3.Dot(transform.forward, wheelVelocityWorld);

            if (motorInput != 0f)
            {
                rb.AddForceAtPosition(transform.forward * motorInput, hit.point);
            }
            else
            {
                rb.AddForceAtPosition(transform.forward * -accelVel * 2f, hit.point);
            }            

            //Tire mesh positions
            if (hit.distance - wheelRadius < restLenght)
            {
                wheelMesh.position = hit.point + transform.up * wheelRadius;
            }
            else
            {
                wheelMesh.localPosition = new Vector3(0f, -restLenght, 0f);
            }

            // Tire Rotation
            float linearVelocity = wheelVelocityLocal.z; // Assuming z-axis is the forward direction

            // Calculate angular velocity (rotation speed) based on linear velocity
            float wheelCircumference = 2 * Mathf.PI * wheelRadius;
            float angularVelocity = linearVelocity / wheelCircumference;

            // Rotate the wheel around its local X-axis based on the calculated angular velocity
            wheelMesh.Rotate(Vector3.right, angularVelocity * Time.fixedDeltaTime * Mathf.Rad2Deg * rotationSpeed);

        }
        else
        {
            hasHit = false;
            wheelMesh.localPosition = new Vector3(0f, -restLenght, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * (maxLength + wheelRadius));

        // Display wheel position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position - transform.up * springDistance, wheelRadius);

        // Display wheel resting position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position - transform.up * restLenght, 0.1f);

        // Display spring length
        Vector3 suspensionTop = transform.position;
        Vector3 suspensionBottom = transform.position - transform.up * springDistance;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(suspensionTop, suspensionBottom);
    }
}
