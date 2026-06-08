using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GateFire : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _isFireBool = "IsFire";
    [Tooltip("Fallback length (seconds) to wait for the FireGone clip before hiding the fire, " +
             "used only if the clip can't be found automatically.")]
    [SerializeField] private float _fireGoneFallback = 1f;

    private int _isFireHash;
    private float _fireGoneLength;
    private Coroutine _routine;

    void Awake()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        _isFireHash = Animator.StringToHash(_isFireBool);
        _fireGoneLength = GetClipLength("FireGone");
        if (_fireGoneLength <= 0f) _fireGoneLength = _fireGoneFallback;
    }

    public void Ignite(float duration)
    {
        gameObject.SetActive(true);

        _animator.SetBool(_isFireHash, true);

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(BurnRoutine(duration));
    }

    private IEnumerator BurnRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        _animator.SetBool(_isFireHash, false);
        yield return new WaitForSeconds(_fireGoneLength);

        _routine = null;
        gameObject.SetActive(false);
    }

    private float GetClipLength(string clipName)
    {
        if (_animator == null || _animator.runtimeAnimatorController == null) return 0f;

        foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            if (clip != null && clip.name == clipName) return clip.length;

        return 0f;
    }
}
