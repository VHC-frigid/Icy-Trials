using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class RBCharacterController : MonoBehaviour
{
    private Rigidbody _rb;
    public Transform cameraPivot;

    public Transform modelMesh;
    public Vector3 groundCheckOffset; //offset was refactored to groundCheckOffset
    public float walkSpeed = 500f, runSpeed = 1000f;
    public float speed = 500f, goalSpeed = 0f;
    public float airSpeed = 10f;
    public float jump = 5f;
    public float groundCheckRadius;
    public float groundCHeckDistance;
    public float coyoteTime = 0.5f;
    public float jumpeBufferTime = 0.25f;
    public float modelLerp = 5, speedLerp = 5;

    private Animator anim;
    //private Vector3 movementVector;
    private Vector3 _input;
    private Vector3 camerPivot;
    private Vector3 _velocity;
    private Vector3 playerDirection;
    private bool _isGrounded;
    private float _coyoteTimer = float.NegativeInfinity;
    private bool _isJumping = false;
    private bool _jumpBuffer = false;
    private float _jumpBufferTimer = 0f;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        playerDirection = transform.forward;
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
        
        playerDirection = Vector3.Slerp(playerDirection, _input.magnitude > 0 ? _input : playerDirection, modelLerp * Time.deltaTime);
        modelMesh.transform.rotation = Quaternion.LookRotation(playerDirection);
        
        if (_input.magnitude > 1)
        {
            _input.Normalize();
        }
        
        goalSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        anim.SetBool("isSprinting?", Input.GetKey(KeyCode.LeftShift));
        speed = Mathf.Lerp(speed, goalSpeed, speedLerp * Time.deltaTime);
        _input *= (speed * Time.deltaTime);
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
        anim.SetBool("isWalking?", _input.magnitude > 0);
    }

    private void FixedUpdate()
    {
        _isGrounded = CheckGrounded();

        if (_isGrounded)
        {
            anim.SetBool("isGrounded?", true);
            //Debug.Log("grounded");
            _rb.velocity = Vector3.SmoothDamp(_rb.velocity, new Vector3(_input.x, _rb.velocity.y, _input.z),
                ref _velocity, 0.1f);
        }
        else //Air Control
        {
            anim.SetBool("isGrounded?", false);
            _rb.AddForce(new Vector3(_input.x,0f,_input.z) * 1.6f, ForceMode.Acceleration);
            if (_rb.velocity.sqrMagnitude > airSpeed * airSpeed)
            {
                //Debug.Log("in air");
                _rb.velocity = _rb.velocity.normalized * airSpeed;
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
