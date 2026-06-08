using UnityEngine;
using UnityEngine.UI;

public class ParryBarUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _parrySprites;
    [SerializeField] private Image _parryImage;

    [SerializeField] private RectTransform _enemyMarker;
    [SerializeField] private bool _manualMarkerRange = false;
    [SerializeField] private float _markerMinX = 0f;
    [SerializeField] private float _markerMaxX = 27f;
    [SerializeField] private PlayerStats _ps;

    void Awake()
    {
        if (_ps == null)
            _ps = FindObjectOfType<PlayerStats>();

        Show(false);
    }

    void Update()
    {
        UpdateParryUI();
    }

    public void UpdateParryUI()
    {
        if (_ps == null || _parryImage == null || _parrySprites.Length == 0) return;

        if (!_ps._isParrying)
        {
            Show(false);
            return;
        }

        Show(true);

        float green = Mathf.Clamp01(_ps._parryGreen);
        int spriteIndex = Mathf.RoundToInt(green * (_parrySprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, _parrySprites.Length - 1);
        _parryImage.sprite = _parrySprites[spriteIndex];

        if (_enemyMarker != null)
        {
            float marker = Mathf.Clamp01(_ps._parryMarker);

            float minX, maxX;
            if (!_manualMarkerRange && _parryImage != null)
            {
                RectTransform bar = _parryImage.rectTransform;
                float halfBar = bar.rect.width * 0.5f;
                float halfMarker = _enemyMarker.rect.width * 0.5f;
                minX = bar.anchoredPosition.x - halfBar + halfMarker;
                maxX = bar.anchoredPosition.x + halfBar - halfMarker;
            }
            else
            {
                minX = _markerMinX;
                maxX = _markerMaxX;
            }

            Vector2 p = _enemyMarker.anchoredPosition;
            p.x = Mathf.Lerp(minX, maxX, marker);
            _enemyMarker.anchoredPosition = p;
        }
    }

    private void Show(bool visible)
    {
        if (_parryImage != null && _parryImage.enabled != visible)
            _parryImage.enabled = visible;

        if (_enemyMarker != null)
        {
            var markerImage = _enemyMarker.GetComponent<Image>();
            if (markerImage != null && markerImage.enabled != visible)
                markerImage.enabled = visible;
        }
    }
}
