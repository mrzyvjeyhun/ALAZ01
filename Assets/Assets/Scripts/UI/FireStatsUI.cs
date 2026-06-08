using UnityEngine;
using UnityEngine.UI;

public class FireStatsUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _fireStatSprites;
    [SerializeField] private Image _fireStatImage;
    private FireDamage _fireDamage;

    void Awake()
    {
        _fireDamage = GetComponent<FireDamage>();
    }

    void Update()
    {
        UpdateFireStatsUI();
    }

    public void UpdateFireStatsUI()
    {
        float fireStatPercent = _fireDamage._health / _fireDamage._maxHealth;

        int spriteIndex = Mathf.RoundToInt(fireStatPercent * (_fireStatSprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, _fireStatSprites.Length -1);

        _fireStatImage.sprite = _fireStatSprites[spriteIndex];
    }
}
