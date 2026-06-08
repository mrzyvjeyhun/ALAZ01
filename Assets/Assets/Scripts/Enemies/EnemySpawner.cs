using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnSettings
    {
        [Header("General")]
        [Tooltip("Master on/off switch for this enemy type.")]
        public bool _enabled = true;

        [Tooltip("Prefab spawned for this type.")]
        public GameObject _prefab;

        [Header("Timing  (lower interval = spawns more often)")]
        [Tooltip("Seconds to wait before this type starts spawning at all.")]
        public float _startDelay = 0f;

        [Tooltip("Shortest random wait between spawn attempts (seconds).")]
        public float _minSpawnInterval = 2f;

        [Tooltip("Longest random wait between spawn attempts (seconds).")]
        public float _maxSpawnInterval = 5f;

        [Header("Amount")]
        [Range(0f, 1f)]
        [Tooltip("Chance (0-1) that a spawn attempt actually spawns. Lower = fewer.")]
        public float _spawnChance = 1f;

        [Tooltip("Fewest spawned at once on a successful attempt. (Incubus ignores this; always 1.)")]
        public int _minPerSpawn = 1;

        [Tooltip("Most spawned at once on a successful attempt. (Incubus ignores this; always 1.)")]
        public int _maxPerSpawn = 1;

        [Tooltip("Max of this type alive at once. 0 = unlimited. (Incubus is always capped at 1.)")]
        public int _maxAlive = 0;

        [Header("Placement")]
        [Tooltip("Random position offset at spawn, so a burst doesn't stack on one spot.")]
        public Vector2 _positionJitter = Vector2.zero;
    }

    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Transform _fireWaypoint;
    [SerializeField] private bool _avoidSameSpawnPoint = true;

    [SerializeField] private SpawnSettings _enemySettings;
    [SerializeField] private SpawnSettings _incubusSettings;

    [SerializeField] private bool _spawnOnStart = true;
    [SerializeField] private float _minGapBetweenSpawns = 0.5f;
    [SerializeField] private FireDamage _fireDamage;

    [SerializeField] private Light2D[] _doorLights;
    [SerializeField] private float _lightDuration = 1.5f;
    [SerializeField] private float _lightBlinkInterval = 0.2f;
    [SerializeField] private float _lightMaxIntensity = 1.5f;
    [SerializeField] private float _delayAfterLight = 0.5f;

    private Coroutine _enemyLoop;
    private Coroutine _incubusLoop;
    private float _lastSpawnTime = -999f;
    private int _lastSpawnPointIndex = -1;
    private readonly HashSet<Transform> _blockedPoints = new HashSet<Transform>();
    private bool _incubusWarningActive;

    void Awake()
    {
        if (_fireDamage == null) _fireDamage = FindObjectOfType<FireDamage>();

        if (_doorLights != null)
        {
            for (int i = 0; i < _doorLights.Length; i++)
            {
                if (_doorLights[i] == null) continue;
                _doorLights[i].enabled = true;
            }
        }
    }

    void Start()
    {
        if (_spawnOnStart)
            StartSpawning();
    }

    public void StartSpawning()
    {
        if (_enemyLoop == null && _enemySettings._enabled)
            _enemyLoop = StartCoroutine(SpawnLoop(_enemySettings));
        if (_incubusLoop == null && _incubusSettings._enabled)
            _incubusLoop = StartCoroutine(SpawnLoop(_incubusSettings));
    }

    public void StopSpawning()
    {
        if (_enemyLoop != null)
        {
            StopCoroutine(_enemyLoop);
            _enemyLoop = null;
        }
        if (_incubusLoop != null)
        {
            StopCoroutine(_incubusLoop);
            _incubusLoop = null;
        }
    }

    public bool IsPointBlocked(Transform point) => point != null && _blockedPoints.Contains(point);

    public void DisableDoor(GameObject door, Transform spawnPoint, float duration)
    {
        StartCoroutine(DisableDoorRoutine(door, spawnPoint, duration));
    }

    private IEnumerator DisableDoorRoutine(GameObject door, Transform spawnPoint, float duration)
    {
        if (spawnPoint != null) _blockedPoints.Add(spawnPoint);
        if (door != null) door.SetActive(false);

        yield return new WaitForSeconds(duration);

        if (door != null) door.SetActive(true);
        if (spawnPoint != null) _blockedPoints.Remove(spawnPoint);
    }

    private IEnumerator SpawnLoop(SpawnSettings settings)
    {
        if (settings._startDelay > 0f)
            yield return new WaitForSeconds(settings._startDelay);

        while (true)
        {
            float wait = Random.Range(settings._minSpawnInterval, settings._maxSpawnInterval);
            yield return new WaitForSeconds(wait);

            if (Random.value > settings._spawnChance) continue;

            while (Time.time - _lastSpawnTime < _minGapBetweenSpawns)
                yield return null;

            _lastSpawnTime = Time.time;
            SpawnEnemy(settings);
        }
    }

    public void SpawnEnemy(SpawnSettings settings)
    {
        if (settings._prefab == null || _spawnPoints.Length == 0) return;

        if (_fireDamage != null && _fireDamage._isDead) return;

        bool isIncubus = settings == _incubusSettings;

        if (!isIncubus && (_incubusWarningActive || incibusAI.AnyAlive)) return;

        if (isIncubus)
        {
            if (_incubusWarningActive || incibusAI.AnyAlive) return;

            Transform incubusPoint = PickSpawnPoint();
            if (incubusPoint == null) return;

            StartCoroutine(SpawnIncubusWithWarning(settings, incubusPoint));
            return;
        }

        int min = Mathf.Max(1, settings._minPerSpawn);
        int max = Mathf.Max(min, settings._maxPerSpawn);
        int count = Random.Range(min, max + 1);

        for (int i = 0; i < count; i++)
        {
            if (settings._maxAlive > 0 && EnemyAI.AliveCount >= settings._maxAlive) break;

            Transform point = PickSpawnPoint();
            if (point == null) break;

            InstantiateEnemy(settings, point);
        }
    }

    private IEnumerator SpawnIncubusWithWarning(SpawnSettings settings, Transform point)
    {
        _incubusWarningActive = true;

        int doorIndex = System.Array.IndexOf(_spawnPoints, point);
        bool hasLight = doorIndex >= 0 && _doorLights != null &&
                        doorIndex < _doorLights.Length && _doorLights[doorIndex] != null;

        if (hasLight)
            yield return BlinkDoorLight(doorIndex);
        else
            yield return new WaitForSeconds(_lightDuration);

        if (_delayAfterLight > 0f)
            yield return new WaitForSeconds(_delayAfterLight);

        if (_blockedPoints.Contains(point)) { _incubusWarningActive = false; yield break; }

        if (_fireDamage != null && _fireDamage._isDead) { _incubusWarningActive = false; yield break; }

        if (incibusAI.AnyAlive) { _incubusWarningActive = false; yield break; }

        InstantiateEnemy(settings, point);
        _incubusWarningActive = false;
    }

    private GameObject InstantiateEnemy(SpawnSettings settings, Transform point)
    {
        Vector3 jitter = new Vector3(
            Random.Range(-settings._positionJitter.x, settings._positionJitter.x),
            Random.Range(-settings._positionJitter.y, settings._positionJitter.y),
            0f);

        GameObject enemy = Instantiate(settings._prefab, point.position + jitter, Quaternion.identity);

        AssignFireWaypoint(enemy, point);
        return enemy;
    }

    private IEnumerator BlinkDoorLight(int index)
    {
        Light2D light = _doorLights[index];
        light.enabled = true;

        float baseIntensity = light.intensity;  

        float t = 0f;
        float blinkTimer = 0f;
        bool on = true;
        light.intensity = _lightMaxIntensity;

        while (t < _lightDuration)
        {
            t += Time.deltaTime;
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= _lightBlinkInterval)
            {
                blinkTimer -= _lightBlinkInterval;
                on = !on;
                light.intensity = on ? _lightMaxIntensity : 0f;
            }

            yield return null;
        }

        light.intensity = baseIntensity;  
    }

    private Transform PickSpawnPoint()
    {
        List<int> available = new List<int>();
        for (int i = 0; i < _spawnPoints.Length; i++)
            if (!_blockedPoints.Contains(_spawnPoints[i]))
                available.Add(i);

        if (available.Count == 0) return null;
        if (available.Count == 1) return _spawnPoints[available[0]];

        int index = available[Random.Range(0, available.Count)];
        if (_avoidSameSpawnPoint && index == _lastSpawnPointIndex)
            index = available[(available.IndexOf(index) + 1) % available.Count];

        _lastSpawnPointIndex = index;
        return _spawnPoints[index];
    }

    private void AssignFireWaypoint(GameObject enemy, Transform spawnPoint)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null && _fireWaypoint != null)
            enemyAI.SetPath(new Transform[] { spawnPoint, _fireWaypoint });

        incibusAI incubus = enemy.GetComponent<incibusAI>();
        if (incubus != null)
        {
            if (_fireWaypoint != null) incubus.SetWaypoint(_fireWaypoint);
            incubus.SetDoor(this, spawnPoint);
        }
    }
}
