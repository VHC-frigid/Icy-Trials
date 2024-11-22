using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RBCharacterController : MonoBehaviour
{
    private Rigidbody _rb;
    public Transform cameraPivot;

    public float speed = 10f;
    public float maxSpeed = 10f;
    public float jump = 5f;
    public float groundCheckRadius;
    public float groundCHeckDistance;
    public float coyoteTime = 0.5f;
    public float jumpeBufferTime = 0.25f;

    //private Vector3 movementVector;
    private Vector3 _input;
    private bool _isGrounded;
    private float _coyoteTimer = float.NegativeInfinity;
    private bool _isJumping = false;
    public Vector3 groundCheckOffset; //offset was refactored to groundCheckOffset
    private Vector3 _velocity;
    private bool _jumpBuffer = false;
    private float _jumpBufferTimer = 0f;
    private Vector3 camerPivot;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //cameraPivot = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementInput();
        JumpInput();
        
        Vector3 forwardFlat = new Vector3(cameraPivot.forward.x, 0, cameraPivot.forward.z).normalized;
        Vector3 sideFlat = new Vector3(cameraPivot.right.x, 0, cameraPivot.right.z).normalized;

        //Define our movement vector based on Player input. Normalise to avoid diagonal exploits
        _input = (forwardFlat * Input.GetAxis("Vertical")) + (sideFlat * Input.GetAxis("Horizontal"));
        _input = Vector3.ClampMagnitude(_input, 1);
    }

    private void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isGrounded || Time.time - coyoteTime < _coyoteTimer)
            {
                Jump();
            }
            else //jump buffer
            {
                _jumpBuffer = true;
                _jumpBufferTimer = Time.time;
            }
        }
        else if (_jumpBuffer && _isGrounded)
        {
            Jump();

            _jumpBuffer = false;
        }

        if (_jumpBuffer)
        {
            if (Time.time - _jumpBufferTimer > jumpeBufferTime)
            {
                _jumpBuffer = false;
                _isJumping = false;
            }
        }
    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * jump, ForceMode.VelocityChange);
        _coyoteTimer = float.NegativeInfinity;
        _isJumping = true;
    }

    private void MovementInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        _isGrounded = CheckGrounded();

        if (_input.magnitude > 1)
        {
            _input.Normalize();
        }

        _input *= (speed * Time.deltaTime);

        if (_isGrounded)
        {
            _rb.velocity = Vector3.SmoothDamp(_rb.velocity, new Vector3(_input.x, _rb.velocity.y, _input.z),
                ref _velocity, 0.1f);
        }
        else //Air Control
        {
            _rb.AddForce(new Vector3(_input.x,0f,_input.z) * 1.6f, ForceMode.Acceleration);
            if (_rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * maxSpeed;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position +groundCheckOffset, groundCheckRadius);
        Gizmos.DrawSphere(transform.position + groundCheckOffset + (Vector3.down * groundCHeckDistance), groundCheckRadius);
    }

    private bool CheckGrounded()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position + groundCheckOffset, groundCheckRadius, Vector3.down, out hit,
                groundCHeckDistance))
        {
            if (_isJumping)
                return false;

            _coyoteTimer = Time.time;
            return true;
        }
        _isJumping = false;
        return false;
    }
}
