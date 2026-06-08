using UnityEngine;

public class CompanionAttack : MonoBehaviour
{
    [SerializeField] private CompanionRangeManager _rangeManager;
    [SerializeField] private LayerMask _enemyMask = ~0;
    [SerializeField] private float _scanInterval = 0.1f;
    [SerializeField] private Transform _bow;
    [SerializeField] private Transform _bowPivot;
    [SerializeField] private float _orbitRadius = -1f;
    [SerializeField] private float _aimAngleOffset = 0f;
    [SerializeField] private float _turnSpeed = 360f;
    [SerializeField] private Arrow _arrowPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _aimDelay = 0.5f;
    [SerializeField] private float _reloadTime = 1f;
    [SerializeField] private Animator _bowAnimator;
    [SerializeField] private string _reloadBool = "IsReloading";
    [SerializeField] private BowAudio _bowAudio;
    public Transform _target { get; private set; }
    private readonly Collider2D[] _hits = new Collider2D[16];
    private ContactFilter2D _filter;
    private float _nextScanTime;
    private float _nextFireTime;
    private bool _hasTarget;
    private float _bowAngle;
    private float _reloadStartTime;
    private bool _reloadPlayed;
    private int _reloadHash;

    void Awake()
    {
        if (_rangeManager == null) _rangeManager = FindObjectOfType<CompanionRangeManager>();
        if (_bow == null) _bow = transform;
        if (_bowPivot == null) _bowPivot = transform;

        if (_orbitRadius < 0f) _orbitRadius = Vector2.Distance(_bow.position, _bowPivot.position);

        _bowAngle = _bow.eulerAngles.z;

        if (_bowAnimator == null && _bow != null) _bowAnimator = _bow.GetComponentInChildren<Animator>();
        _reloadHash = Animator.StringToHash(_reloadBool);

        if (_bowAudio == null) _bowAudio = GetComponentInChildren<BowAudio>();

        _filter = new ContactFilter2D
        {
            useTriggers = true
        };
        _filter.SetLayerMask(_enemyMask);
    }

    void Update()
    {
        if (Time.time >= _nextScanTime)
        {
            _nextScanTime = Time.time + _scanInterval;
            _target = AcquireTarget();
        }

        AimBow();
        TryShoot();
    }

    private void TryShoot()
    {
        if (_arrowPrefab == null || _target == null)
        {
            _hasTarget = false;
            if (_bowAnimator != null) _bowAnimator.SetBool(_reloadHash, false);
            return;
        }

        if (!_hasTarget)
        {
            _hasTarget = true;
            BeginReload(Time.time + _aimDelay);
        }

        if (!_reloadPlayed && Time.time >= _reloadStartTime)
        {
            _reloadPlayed = true;
            if (_bowAnimator != null) _bowAnimator.SetBool(_reloadHash, true);
        }

        if (Time.time < _nextFireTime) return;

        if (_bowAnimator != null) _bowAnimator.SetBool(_reloadHash, false);

        Transform spawn = _firePoint != null ? _firePoint : _bow;
        Vector2 dir = (Vector2)_target.position - (Vector2)spawn.position;

        Arrow arrow = Instantiate(_arrowPrefab, spawn.position, Quaternion.identity);
        arrow.Launch(dir);

        if (_bowAudio != null) _bowAudio.PlayShootSFX();

        BeginReload(Time.time);
    }

    private void BeginReload(float startTime)
    {
        _reloadStartTime = startTime;
        _nextFireTime = startTime + _reloadTime;
        _reloadPlayed = false;
    }

    private void AimBow()
    {
        if (_bow == null || _target == null) return;

        Vector2 pivot = _bowPivot.position;
        Vector2 dir = (Vector2)_target.position - pivot;
        if (dir.sqrMagnitude < 0.0001f) return;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _bowAngle = Mathf.MoveTowardsAngle(_bowAngle, targetAngle, _turnSpeed * Time.deltaTime);

        float rad = _bowAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * _orbitRadius;
        _bow.position = pivot + offset;
        _bow.rotation = Quaternion.Euler(0f, 0f, _bowAngle + _aimAngleOffset);
    }

    private Transform AcquireTarget()
    {
        BoxCollider2D range = _rangeManager != null ? _rangeManager.ActiveRange : null;
        if (range == null || !range.enabled) return null;

        int count = range.Overlap(_filter, _hits);
        if (count == 0) return null;

        Transform nearest = null;
        float bestSqr = float.MaxValue;
        Vector2 origin = transform.position;

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = _hits[i];
            if (hit == null || !IsEnemy(hit)) continue;

            float sqr = ((Vector2)hit.transform.position - origin).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    private bool IsEnemy(Collider2D col)
    {
        return col.GetComponentInParent<EnemyAI>() != null
            || col.GetComponentInParent<incibusAI>() != null;
    }
}
