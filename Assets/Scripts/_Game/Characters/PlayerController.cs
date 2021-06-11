using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //This Object gets set from the Level Manager
    public TransformAnchor gameplayCameraTransform = default;

    //This is just so we can easily what the positon we want is 
    [SerializeField] Transform groundCheckTransform = default;

    [SerializeField] private InputReader inputReader;
    [SerializeField] private CharacterController characterController;

    [Header("Constants")]
    [SerializeField] private FloatVariable globalGravity;

    [Header("Customizables")]
    [SerializeField] private FloatVariable maxSpeed;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool test;

    [NonSerialized] public Vector3 movementInput;

    private Vector2 _inputVector;
    private bool _isRunning;
    private bool _jumpInput;
    private bool _isGrounded = false;
    private float _previousSpeed;

    public bool _isPosessed = false;

    private Vector3 _gravitySpeed = Vector3.zero;

    public bool IsPosessed { get => _isPosessed; }

    private void Awake()
    {
        inputReader.moveEvent += OnMove;
        inputReader.jumpEvent += JumpInitiated;
        inputReader.jumpCanceledEvent += JumpEnded;
        inputReader.startRunningEvent += OnRunStarted;
        inputReader.stopRunningEvent += OnRunStopped;

        gameplayCameraTransform.Transform = Camera.main.transform;
    }

    private void OnDestroy()
    {
        inputReader.moveEvent -= OnMove;
        inputReader.jumpEvent -= JumpInitiated;
        inputReader.jumpCanceledEvent -= JumpEnded;
        inputReader.startRunningEvent -= OnRunStarted;
        inputReader.stopRunningEvent -= OnRunStopped;
    }

    private void JumpInitiated() { _jumpInput = true; }

    private void JumpEnded() { _jumpInput = false; }

    private void OnRunStarted() { _isRunning = true; }

    private void OnRunStopped() { _isRunning = false; }

    private void OnMove(Vector2 movement) { _inputVector = movement; }

    private void Update()
    {
        if (!test)
        {
            if (_isPosessed)
            {
                ProcessMovement();
            }

            GroundCheck();
            HandleGravity();

            HandleMovement();
        }
        else
        {
            if (_isPosessed)
            {
                ProcessMovementForRigidbody();
                GroundCheck();
            }
        }
    }

    private void FixedUpdate()
    {
        _rb.AddForce(movementInput, ForceMode.Force);
    }


    private void ProcessMovementForRigidbody()
    {
        float targetSpeed = 0f;
        Vector3 adjustedMovement;

        if (gameplayCameraTransform.isSet)
        {
            Vector3 cameraForward = gameplayCameraTransform.Transform.forward;
            cameraForward.y = 0;
            Vector3 cameraRight = gameplayCameraTransform.Transform.right;
            cameraRight.y = 0;
            adjustedMovement = cameraRight.normalized * _inputVector.x + cameraForward.normalized * _inputVector.y;
        }
        else { adjustedMovement = new Vector3(_inputVector.x, 0f, _inputVector.y); }

        targetSpeed = Mathf.Clamp01(_inputVector.magnitude);
        if (targetSpeed > 0f)
        {
            if (_isRunning)
                targetSpeed = 1f;
        }
        targetSpeed = Mathf.Lerp(_previousSpeed, targetSpeed, Time.deltaTime * 4);

        movementInput = adjustedMovement.normalized * targetSpeed * maxSpeed.Value;
        _previousSpeed = targetSpeed;
    }

    private void ProcessMovement()
    {
        float targetSpeed = 0f;
        Vector3 adjustedMovement;

        if (gameplayCameraTransform.isSet)
        {
            Vector3 cameraForward = gameplayCameraTransform.Transform.forward;
            cameraForward.y = 0;
            Vector3 cameraRight = gameplayCameraTransform.Transform.right;
            cameraRight.y = 0;
            adjustedMovement = cameraRight.normalized * _inputVector.x + cameraForward.normalized * _inputVector.y;
        }
        else { adjustedMovement = new Vector3(_inputVector.x, 0f, _inputVector.y); }


        targetSpeed = Mathf.Clamp01(_inputVector.magnitude);
        if (targetSpeed > 0f)
        {
            if (_isRunning)
                targetSpeed = 1f;
        }
        targetSpeed = Mathf.Lerp(_previousSpeed, targetSpeed, Time.deltaTime * 4);

        movementInput = adjustedMovement.normalized * targetSpeed * maxSpeed.Value;

        _previousSpeed = targetSpeed;

    }

    private void GroundCheck()
    {
        Collider[] overlappedObjs = Physics.OverlapSphere(groundCheckTransform.transform.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        _isGrounded = overlappedObjs.Length >= 1 ? true : false;

        if (_isGrounded)
        {
            _gravitySpeed = Vector3.zero;
        }

    }

    private void HandleGravity()
    {
        if (!_isGrounded) { _gravitySpeed += -Physics.gravity.normalized * globalGravity.Value * Time.deltaTime; }
        movementInput += _gravitySpeed;
    }

    private void HandleMovement()
    {
        characterController.Move(movementInput * Time.deltaTime);
    }

    public virtual void PosessCharacter(bool state)
    {
        _isPosessed = state;

        if (!_isPosessed) { movementInput = Vector3.zero; }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (Application.isPlaying)
        {
            if (!_isGrounded) { Gizmos.color = Color.green; }

            Gizmos.DrawWireSphere(groundCheckTransform.transform.position, groundCheckRadius);

        }
    }

}
