using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float movementStrenght;
    private float vertical;
    private float horizontal;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");

    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.right * horizontal * movementStrenght, ForceMode.Force);
    }
}
