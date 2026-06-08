using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class CompanionMovement : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    private CompanionRangeManager _crm;
    private Transform _transform;
    private Animator _anim;
    [SerializeField] private CompanionAudio _companionAudio;

    [SerializeField] Vector2 _upTeleport;
    [SerializeField] Vector2 _downTeleport;
    [SerializeField] Vector2 _rightTeleport;
    [SerializeField] Vector2 _leftTeleport;
    [SerializeField] public bool _isUp;
    [SerializeField] public bool _isDown;
    [SerializeField] public bool _isRight;
    [SerializeField] public bool _isLeft;

    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private float _fadeDuration = 0.12f;
    [SerializeField] private Light2D _teleportLightPrefab;
    [SerializeField] private float _lightFadeDuration = 0.3f;

    private Coroutine _teleportRoutine;
    private Vector2 _teleportTarget;

    void Awake()
    {
        _inputActions = new InputSystem_Actions();

        if (_crm == null) _crm = FindObjectOfType<CompanionRangeManager>();
        _anim = GetComponent<Animator>();

        _transform = GetComponent<Transform>();
        if (_sprite == null) _sprite = GetComponentInChildren<SpriteRenderer>();
        if (_companionAudio == null) _companionAudio = GetComponent<CompanionAudio>();
    }

    void Start()
    {
        _isUp = false;
        _isDown = false;
        _isRight = false;
        _isLeft = true;

        _transform.position = _leftTeleport;   
        _teleportTarget = _leftTeleport;
    }

    void OnEnable()
    {
        _inputActions.Player.Up.performed += OnUpTeleport;
        _inputActions.Player.Down.performed += OnDownTeleport;
        _inputActions.Player.Right.performed += OnRightTeleport;
        _inputActions.Player.Left.performed += OnLeftTeleport;
        _inputActions.Enable();
    }

    void OnDisable()
    {
        _inputActions.Player.Up.performed -= OnUpTeleport;
        _inputActions.Player.Down.performed -= OnDownTeleport;
        _inputActions.Player.Right.performed -= OnRightTeleport;
        _inputActions.Player.Left.performed -= OnLeftTeleport;
        _inputActions.Disable();
    }

    private void OnUpTeleport(InputAction.CallbackContext context)
    {
        if (_isDown)
        {
            _isDown = false;
            _anim.SetBool("IsUpRight", _isDown);
            _crm.DownDisable();
        }
        if (_isRight)
        {
            _isRight = false;
            _anim.SetBool("IsUpLeft", _isRight);
            _crm.RightDisable();
        }
        if (_isLeft)
        {
            _isLeft = false;
            _anim.SetBool("IsRight", _isLeft);
            _crm.LeftDisable();
        }

        Teleport(_upTeleport);
        _isUp = true;
        _anim.SetBool("IsLeft", _isUp);
        _crm.UpEnable();

        if (context.performed)

        Debug.Log("up");
    }

    private void OnDownTeleport(InputAction.CallbackContext context)
    {
        if (_isUp)
        {
            _isUp = false;
            _anim.SetBool("IsLeft", _isUp);
            _crm.UpDisable();
        }
        if (_isRight)
        {
            _isRight = false;
            _anim.SetBool("IsUpLeft", _isRight);
            _crm.RightDisable();
        }
        if (_isLeft)
        {
            _isLeft = false;
            _anim.SetBool("IsRight", _isLeft);
            _crm.LeftDisable();
        }

        Teleport(_downTeleport);

        _isDown = true;
        _anim.SetBool("IsUpRight", _isDown);
        _crm.DownEnable();

        Debug.Log("down");
    }

    private void OnRightTeleport(InputAction.CallbackContext context)
    {
        if (_isUp)
        {
            _isUp = false;
            _anim.SetBool("IsLeft", _isUp);
            _crm.UpDisable();
        }
        if (_isDown)
        {
            _isDown = false;
            _anim.SetBool("IsUpRight", _isDown);
            _crm.DownDisable();
        }
        if (_isLeft)
        {
            _isLeft = false;
            _anim.SetBool("IsRight", _isLeft);
            _crm.LeftDisable();
        }

        Teleport(_rightTeleport);

        _isRight = true;
        _anim.SetBool("IsUpLeft", _isRight);
        _crm.RightEnable();

        Debug.Log("right");
    }
    
    private void OnLeftTeleport(InputAction.CallbackContext context)
    {
        if (_isUp)
        {
            _isUp = false;
            _anim.SetBool("IsLeft", _isUp);
            _crm.UpDisable();
        }
        if (_isDown)
        {
            _isDown = false;
            _anim.SetBool("IsUpRight", _isDown);
            _crm.DownDisable();
        }
        if (_isRight)
        {
            _isRight = false;
            _anim.SetBool("IsUpLeft", _isRight);
            _crm.RightDisable();
        }

        Teleport(_leftTeleport);

        _isLeft = true;
        _anim.SetBool("IsRight", _isLeft);
        _crm.LeftEnable();

        Debug.Log("left");
    }

    private void Teleport(Vector2 target)
    {
        if (_teleportRoutine != null)
        {
            StopCoroutine(_teleportRoutine);
            _transform.position = _teleportTarget;
            SetSpriteAlpha(1f);
        }

        _teleportTarget = target;

        if (_companionAudio != null) _companionAudio.PlayTeleportSFX();

        if (isActiveAndEnabled)
            _teleportRoutine = StartCoroutine(TeleportRoutine(target));
        else
            _transform.position = target;
    }

    private IEnumerator TeleportRoutine(Vector2 target)
    {
        Vector2 origin = _transform.position;

        SpawnTeleportLight(origin);
        yield return FadeSprite(1f, 0f);

        _transform.position = target;

        SpawnTeleportLight(target);
        yield return FadeSprite(0f, 1f);

        _teleportRoutine = null;
    }

    private IEnumerator FadeSprite(float from, float to)
    {
        if (_sprite == null || _fadeDuration <= 0f)
        {
            SetSpriteAlpha(to);
            yield break;
        }

        float t = 0f;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            SetSpriteAlpha(Mathf.Lerp(from, to, t / _fadeDuration));
            yield return null;
        }
        SetSpriteAlpha(to);
    }

    private void SetSpriteAlpha(float a)
    {
        if (_sprite == null) return;
        Color c = _sprite.color;
        c.a = a;
        _sprite.color = c;
    }

    private void SpawnTeleportLight(Vector2 position)
    {
        if (_teleportLightPrefab == null) return;
        Light2D light = Instantiate(_teleportLightPrefab, position, Quaternion.identity);
        StartCoroutine(FadeAndDestroyLight(light));
    }

    private IEnumerator FadeAndDestroyLight(Light2D light)
    {
        if (light == null) yield break;

        float start = light.intensity;
        float t = 0f;
        while (t < _lightFadeDuration && light != null)
        {
            t += Time.deltaTime;
            light.intensity = Mathf.Lerp(start, 0f, t / _lightFadeDuration);
            yield return null;
        }
        if (light != null) Destroy(light.gameObject);
    }
}
