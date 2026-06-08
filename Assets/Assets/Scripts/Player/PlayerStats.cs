using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int _maxParry = 3;
    public int _currentParry;
    public bool _canParry;

    public float _maxSwordFire = 100f;
    public float _currentSwordFire;

    [HideInInspector] public bool _isParrying;
    [HideInInspector] public float _parryGreen;
    [HideInInspector] public float _parryMarker;

    void Update()
    {
        if (_currentSwordFire >= _maxSwordFire)
        {
            _canParry = true;
        }
        else
        {
            _canParry = false;
        }
    }

}
