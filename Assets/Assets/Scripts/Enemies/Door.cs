using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private EnemySpawner _spawner;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GateFire _fire;
    [SerializeField] private float _disableDuration = 5f;

    void Awake()
    {
        if (_playerStats == null) _playerStats = FindObjectOfType<PlayerStats>();
        if (_spawner == null) _spawner = FindObjectOfType<EnemySpawner>();
    }

    public void DisableDoor()
    {
        if (_playerStats == null || _spawner == null) return;

        if (_playerStats._currentParry < _playerStats._maxParry) return;

        _playerStats._currentParry = 0;
        _spawner.DisableDoor(gameObject, _spawnPoint, _disableDuration);

        if (_fire != null) _fire.Ignite(_disableDuration);
    }
}
