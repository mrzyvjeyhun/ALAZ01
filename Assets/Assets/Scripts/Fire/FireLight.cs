using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireLight : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private float _flashDelay = 0.1f;
    [SerializeField] private float _flashHold = 0.3f;
    [SerializeField] private float _flashDuration = 1f;
    [SerializeField] private float _flashIntensityMultiplier = 2f;

    private Color _baseColor;
    private float _baseIntensity;
    private Coroutine _flashRoutine;

    void Awake()
    {
        if (_light == null) _light = GetComponentInChildren<Light2D>();
        if (_light != null)
        {
            _baseColor = _light.color;
            _baseIntensity = _light.intensity;
        }
    }

    public void Flash(Color color)
    {
        if (_light == null) return;
        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashRoutine(color));
    }

    private IEnumerator FlashRoutine(Color color)
    {
        if (_flashDelay > 0f)
            yield return new WaitForSeconds(_flashDelay);

        float peak = _baseIntensity * _flashIntensityMultiplier;

        _light.color = color;
        _light.intensity = peak;
        if (_flashHold > 0f)
            yield return new WaitForSeconds(_flashHold);

        float t = 0f;
        while (t < _flashDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / _flashDuration); 
            _light.color = Color.Lerp(color, _baseColor, k);
            _light.intensity = Mathf.Lerp(peak, _baseIntensity, k);
            yield return null;
        }

        _light.color = _baseColor;
        _light.intensity = _baseIntensity;
        _flashRoutine = null;
    }
}
