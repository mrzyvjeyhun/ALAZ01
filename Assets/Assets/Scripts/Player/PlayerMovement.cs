using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    private Rigidbody2D _rb;
    private Animator _anim;
    [SerializeField] private FireDamage _fr;
    [SerializeField] private PlayerAudio _playerAudio;
    private Vector2 _moveInput;
    [SerializeField] private float _speed = 5f;
    private bool _isWalking;
    private Vector2 _lastMoveDirection = Vector2.up;

    // Current facing direction (last non-zero move input), used by the parry sword.
    public Vector2 FacingDirection => _lastMoveDirection;

    void Awake()
    {
       _inputActions = new InputSystem_Actions();

       _rb = GetComponent<Rigidbody2D>();
       _anim = GetComponent<Animator>();
       if (_fr == null) _fr = FindObjectOfType<FireDamage>();
       if (_playerAudio == null) _playerAudio = GetComponent<PlayerAudio>();
    }

    void OnEnable()
    {
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        _inputActions.Enable();
    }

    void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;
        _inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void UpdateAnimations()
    {
        if (_moveInput.sqrMagnitude > 0.01f)
        {
            _lastMoveDirection = _moveInput.normalized;
        }

        bool wasWalking = _isWalking;

        if (_moveInput != Vector2.zero)
        {
            _isWalking = true;
            _anim.SetBool("IsWalking", _isWalking);
        }
        else
        {
            _isWalking = false;
            _anim.SetBool("IsWalking", _isWalking);
        }

        if (_playerAudio != null && _isWalking != wasWalking)
        {
            if (_isWalking) _playerAudio.StartFootsteps();
            else _playerAudio.StopFootsteps();
        }

        _anim.SetFloat("MoveX", _lastMoveDirection.x);
        _anim.SetFloat("MoveY", _lastMoveDirection.y);
    }

    void FixedUpdate()
    {
        if (_fr != null && _fr._isDead)
        {
            _rb.linearVelocity = Vector2.zero;
            if (_isWalking)
            {
                _isWalking = false;
                if (_playerAudio != null) _playerAudio.StopFootsteps();
            }
            return;
        }

        UpdateAnimations();
        _rb.linearVelocity = _speed * _moveInput;
    }
}
