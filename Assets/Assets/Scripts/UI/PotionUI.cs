using UnityEngine;
using UnityEngine.UI;

public class PotionUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _potionSprites;
    [SerializeField] private Image _potionImage;
    [SerializeField] private PlayerStats _ps;

    void Awake()
    {
        if (_ps == null)
            _ps = FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        UpdatePotionUI();
    }

    public void UpdatePotionUI()
    {
        if (_ps == null || _potionSprites.Length == 0) return;

        float fireStatPercent = _ps._maxParry == 0
            ? 0f
            : (float)_ps._currentParry / _ps._maxParry;

        int spriteIndex = Mathf.RoundToInt(fireStatPercent * (_potionSprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, _potionSprites.Length -1);

        _potionImage.sprite = _potionSprites[spriteIndex];
    }
}
