using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
public class incibusAI : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private float _lightDuration = 1.5f;
    [SerializeField] private float _lightSwayDistance = 0.5f;
    [SerializeField] private float _lightMaxIntensity = 1.5f;

    [SerializeField] private Vector2 _moveDirection = Vector2.left;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private Transform _insidePoint;
    [SerializeField] private float _insideReachThreshold = 0.2f;

    [SerializeField] private Transform _parryStartPoint;
    [SerializeField] private Transform _parryEndPoint;

    [SerializeField, Range(0f, 1f)] private float _perfectTolerance = 0.06f;
    [SerializeField] private float _deathFadeDuration = 0.4f;

    private enum State { LightRising, Advancing }
    private State _state = State.LightRising;

    [SerializeField] private BoxCollider2D _bx;
    [SerializeField] private Animator _anim;
    private SpriteRenderer _sr;
    private Rigidbody2D _rb;
    private PlayerStats _ps;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private FireLight _fireLight;
    private FireDamage _fireDamage;
    private ParryAudio _parryAudio;
    private PlayerAudio _playerAudio;
    private SwordParry _swordParry;
    private InputSystem_Actions _input;

    private Vector2 _startPos;  
    private Vector2 _travel;     
    private float _travelLenSq;
    private bool _parryArmed;  
    private bool _parryFailed;   
    private Collider2D _doorZone; 
    private EnemySpawner _spawner;
    private Transform _spawnPoint;

    private static int _aliveCount;
    public static bool AnyAlive => _aliveCount > 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => _aliveCount = 0;

    void Awake()
    {
        _aliveCount++;

        _rb = GetComponent<Rigidbody2D>();
        _bx = GetComponentInChildren<BoxCollider2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        if (_anim == null) _anim = GetComponentInChildren<Animator>();

        _input = new InputSystem_Actions();
    }

    void OnEnable()  => _input.Enable();
    void OnDisable()
    {
        _input.Disable();
        EndParryWindow();
    }

    void OnDestroy()
    {
        _aliveCount = Mathf.Max(0, _aliveCount - 1);
    }

    public void SetWaypoint(Transform waypoint)
    {
        _insidePoint = waypoint;
    }

    public void SetDoor(EnemySpawner spawner, Transform spawnPoint)
    {
        _spawner = spawner;
        _spawnPoint = spawnPoint;
    }

    private bool DoorClosed()
    {
        return _spawner != null && _spawnPoint != null && _spawner.IsPointBlocked(_spawnPoint);
    }

    void Start()
    {
        _ps = FindObjectOfType<PlayerStats>();   
        _fireLight = FindObjectOfType<FireLight>();
        _fireDamage = FindObjectOfType<FireDamage>();
        _parryAudio = FindObjectOfType<ParryAudio>();
        _playerAudio = FindObjectOfType<PlayerAudio>();
        _swordParry = FindObjectOfType<SwordParry>();

        _sr.enabled = false;
        _bx.enabled = false;

        StartCoroutine(RaiseLightThenAdvance());
    }

    private IEnumerator RaiseLightThenAdvance()
    {
        if (_light != null)
        {
            _light.enabled = true;
            float startX = _light.transform.localPosition.x;
            float t = 0f;
            while (t < _lightDuration)
            {
                if (DoorClosed()) { CancelSpawn(); yield break; }

                t += Time.deltaTime;
                float k = t / _lightDuration;

                _light.intensity = Mathf.Lerp(0f, _lightMaxIntensity, Mathf.Sin(k * Mathf.PI));

                Vector3 p = _light.transform.localPosition;
                p.x = startX + Mathf.Sin(k * Mathf.PI) * _lightSwayDistance;
                _light.transform.localPosition = p;
                yield return null;
            }
            _light.enabled = false;
        }
        else
        {
            yield return new WaitForSeconds(_lightDuration);
        }

        if (DoorClosed()) { CancelSpawn(); yield break; }

        _sr.enabled = true;
        _bx.enabled = true;

        CaptureParryRange();
        _state = State.Advancing;
    }

    private void CancelSpawn()
    {
        if (_light != null) _light.enabled = false;
        Destroy(gameObject);
    }

    private void CaptureParryRange()
    {
        Transform start = _parryStartPoint != null ? _parryStartPoint : _spawnPoint;   
        Transform end   = _parryEndPoint   != null ? _parryEndPoint   : _insidePoint; 

        _startPos = start != null ? (Vector2)start.position : (Vector2)transform.position;
        Vector2 endPos = end != null ? (Vector2)end.position : _startPos;
        _travel = endPos - _startPos;
        _travelLenSq = _travel.sqrMagnitude;

        FindDoorZone();
    }

    private void FindDoorZone()
    {
        Transform door = _spawnPoint != null ? _spawnPoint : _parryStartPoint;
        if (door == null) return;

        foreach (var col in door.GetComponentsInChildren<BoxCollider2D>())
        {
            if (col.isTrigger) { _doorZone = col; break; }
        }

        if (_doorZone == null)
            Debug.LogWarning($"[incibusAI] No trigger collider found under door '{door.name}'; " +
                             "the parry bar will not be gated to the door area.", this);
    }

    private float ProjectOntoTravel(Vector2 worldPos)
    {
        if (_travelLenSq <= 0.0001f) return 0f;
        return Mathf.Clamp01(Vector2.Dot(worldPos - _startPos, _travel) / _travelLenSq);
    }

    void FixedUpdate()
    {
        if (_state != State.Advancing)
        {
            SetVelocity(Vector2.zero);
            return;
        }

        if (_insidePoint != null)
        {
            Vector2 toTarget = (Vector2)_insidePoint.position - _rb.position;

            if (toTarget.magnitude <= _insideReachThreshold)
            {
                SetVelocity(Vector2.zero);
                return;
            }

            SetVelocity(toTarget.normalized * _speed);
        }
        else
        {
            SetVelocity(_moveDirection.normalized * _speed);
        }
    }

    private void SetVelocity(Vector2 velocity)
    {
        _rb.linearVelocity = velocity;

        if (_anim == null) return;

        if (velocity.sqrMagnitude > 0.0001f)
        {
            Vector2 dir = velocity.normalized;
            _anim.SetFloat(MoveXHash, dir.x);
            _anim.SetFloat(MoveYHash, dir.y);
        }
    }

    void Update()
    {
        if (_state != State.Advancing || _ps == null) return;

        bool hasRange = _travelLenSq > 0.0001f;

        float marker = hasRange ? ProjectOntoTravel(transform.position) : 0f;

        bool playerInZone = _doorZone == null || _doorZone.OverlapPoint(_ps.transform.position);

        bool open = hasRange && _ps._canParry && playerInZone && marker < 1f && !_parryFailed;

        if (open)
        {
            _parryArmed = true;

            float green = ProjectOntoTravel(_ps.transform.position);

            _ps._isParrying = true;
            _ps._parryGreen = green;
            _ps._parryMarker = marker;

            if (_input.Player.Attack.WasPressedThisFrame())
            {
                if (_playerAudio != null) _playerAudio.PlayParrySFX();

                float diff = marker - green;

                if (diff > _perfectTolerance)
                {
                    _parryFailed = true;
                    FlashFire(Color.red);
                    EndParryWindow();
                }
                else
                {
                    _ps._currentSwordFire = 0f;
                    if (_swordParry != null) _swordParry.Play();
                    if (diff >= -_perfectTolerance)
                    {
                        ++_ps._currentParry;
                        FlashFire(Color.green);
                        if (_parryAudio != null) _parryAudio.PlayPerfectSFX();
                    }
                    else
                    {
                        FlashFire(Color.yellow);
                        if (_parryAudio != null) _parryAudio.PlayYellowSFX();
                        if (_fireDamage != null)
                            _fireDamage.TakeDamage(_fireDamage._incubusDamage * 0.5f);
                    }
                    EndParryWindow();
                    DeathFadeRunner.Kill(gameObject, _deathFadeDuration);
                }
            }
        }
        else if (_parryArmed)
        {
            EndParryWindow();
        }
    }

    private void FlashFire(Color color)
    {
        if (_fireLight != null) _fireLight.Flash(color);
    }

    private void EndParryWindow()
    {
        _parryArmed = false;
        if (_ps == null) return;
        _ps._isParrying = false;
        _ps._parryGreen = 0f;
        _ps._parryMarker = 0f;
    }
}
