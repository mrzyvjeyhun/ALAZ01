using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _reachThreshold = 0.1f;

    private Rigidbody2D _rb;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform[] _waypoints;
    private int _index;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    private static int _aliveCount;
    public static int AliveCount => _aliveCount;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => _aliveCount = 0;

    void Awake()
    {
        _aliveCount++;
        _rb = GetComponent<Rigidbody2D>();
        if (_anim == null) _anim = GetComponentInChildren<Animator>();
    }

    void OnDestroy()
    {
        _aliveCount = Mathf.Max(0, _aliveCount - 1);
    }

    public void SetPath(Transform[] waypoints)
    {
        _waypoints = waypoints;
        _index = 0;

        if (_waypoints != null && _waypoints.Length > 0)
            _rb.position = _waypoints[0].position;
    }

    void FixedUpdate()
    {
        if (_waypoints == null || _index >= _waypoints.Length)
        {
            SetVelocity(Vector2.zero);
            return;
        }

        Vector2 targetPos = _waypoints[_index].position;
        Vector2 toTarget = targetPos - _rb.position;

        if (toTarget.magnitude <= _reachThreshold)
        {
            _index++;
            if (_index >= _waypoints.Length)
            {
                SetVelocity(Vector2.zero);
                return;
            }
            toTarget = (Vector2)_waypoints[_index].position - _rb.position;
        }

        SetVelocity(toTarget.normalized * _speed);
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
}
