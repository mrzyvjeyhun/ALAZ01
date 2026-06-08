using UnityEngine;

public class SwordFireZone : MonoBehaviour
{
    [SerializeField] private float _fillDuration = 5f;

    private PlayerStats _ps;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() == null) return;
        _ps = other.GetComponent<PlayerStats>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() == null) return;
        _ps = null;
    }

    void Update()
    {
        if (_ps == null) return;

        if (_ps._currentSwordFire >= _ps._maxSwordFire)
        {
            _ps._currentSwordFire = _ps._maxSwordFire;
            return;
        }

        float ratePerSecond = _fillDuration > 0f ? _ps._maxSwordFire / _fillDuration : _ps._maxSwordFire;
        _ps._currentSwordFire = Mathf.Min(
            _ps._currentSwordFire + ratePerSecond * Time.deltaTime,
            _ps._maxSwordFire);
    }
}
