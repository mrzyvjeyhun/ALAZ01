using UnityEngine;
using UnityEngine.UI;

public class SwordFireZoneUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _swordFireSprites;
    [SerializeField] private Image _swordFireImage;
    [SerializeField] private PlayerStats _ps;

    void Awake()
    {
        if (_ps == null)
            _ps = FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        UpdateSwordFireUI();
    }

    public void UpdateSwordFireUI()
    {
        if (_ps == null || _swordFireSprites.Length == 0) return;

        float swordFirePercent = _ps._maxSwordFire == 0
            ? 0f
            : _ps._currentSwordFire / _ps._maxSwordFire;

        int spriteIndex = Mathf.RoundToInt(swordFirePercent * (_swordFireSprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, _swordFireSprites.Length -1);

        _swordFireImage.sprite = _swordFireSprites[spriteIndex];
    }
}
