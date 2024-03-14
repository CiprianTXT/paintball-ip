using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSmoothTime = 0.1f;
    public float jumpForce = 7f;
    public Transform cameraTilt;
    public Transform body;

    Rigidbody rb;
    float turnSmoothVelocity;
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // We want to rotate the character ourselves
    }

    void Update()
    {
        // Check if the character is grounded using a collider
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0f, body.localScale.y, 0f), 0.2f, LayerMask.GetMask("Ground"));

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTilt.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y), rb.velocity.z);
        }
    }
}
