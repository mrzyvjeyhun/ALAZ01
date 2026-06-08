using System.Collections;
using UnityEngine;

// Drives the player's parry sword. On a successful parry (yellow or green) the
// sword GameObject is enabled, oriented to face the player's current direction
// (one clip rotated/flipped for all 4 directions), and then disabled again when
// the swing finishes.
//
// The sword's editor position/rotation is treated as the "facing right"
// reference, so the single clip should be authored facing right.
[DisallowMultipleComponent]
public class SwordParry : MonoBehaviour
{
    [Tooltip("The sword GameObject that is toggled on/off for each swing.")]
    [SerializeField] private GameObject _swordObject;
    [Tooltip("Animator on the sword.")]
    [SerializeField] private Animator _swordAnimator;
    [Tooltip("Used to read the player's current facing direction.")]
    [SerializeField] private PlayerMovement _playerMovement;

    [Tooltip("State to play in the sword Animator.")]
    [SerializeField] private string _swingState = "Parry";
    [Tooltip("Fallback swing length (seconds) used if the clip length can't be read.")]
    [SerializeField] private float _swingDuration = 0.35f;
    [Tooltip("Flip the sprite vertically when facing left so the blade isn't upside-down.")]
    [SerializeField] private bool _flipYWhenFacingLeft = true;

    private int _swingStateHash;
    private Coroutine _swingRoutine;

    private SpriteRenderer _swordSprite;
    private Vector3 _baseLocalPos;
    private Quaternion _baseLocalRot;

    void Awake()
    {
        if (_playerMovement == null) _playerMovement = GetComponentInParent<PlayerMovement>();

        if (_swordObject == null)
        {
            Transform sword = transform.Find("Sword");
            if (sword != null) _swordObject = sword.gameObject;
        }
        if (_swordObject != null)
        {
            if (_swordAnimator == null) _swordAnimator = _swordObject.GetComponent<Animator>();
            _swordSprite = _swordObject.GetComponent<SpriteRenderer>();

            // The editor placement is the "facing right" reference pose.
            _baseLocalPos = _swordObject.transform.localPosition;
            _baseLocalRot = _swordObject.transform.localRotation;
        }

        _swingStateHash = Animator.StringToHash(_swingState);

        // Hidden until a parry triggers a swing.
        if (_swordObject != null) _swordObject.SetActive(false);
    }

    // Call on a successful parry (yellow or green) to swing the sword.
    public void Play()
    {
        if (_swordObject == null || _swordAnimator == null) return;

        if (_swingRoutine != null) StopCoroutine(_swingRoutine);
        _swingRoutine = StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        Vector2 dir = GetFacing();

        _swordObject.SetActive(true);
        Orient(dir);
        _swordAnimator.Play(_swingStateHash, 0, 0f);

        // Let the animator evaluate the freshly-started state before reading it.
        yield return null;

        AnimatorStateInfo info = _swordAnimator.GetCurrentAnimatorStateInfo(0);
        float wait = info.length > 0f ? info.length : _swingDuration;

        yield return new WaitForSeconds(wait);

        _swordObject.SetActive(false);
        _swingRoutine = null;
    }

    // Rotate the reference (right-facing) pose around the player to face dir.
    private void Orient(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        Quaternion turn = Quaternion.Euler(0f, 0f, angle);

        Transform t = _swordObject.transform;
        t.localPosition = turn * _baseLocalPos;
        t.localRotation = turn * _baseLocalRot;

        if (_swordSprite != null)
            _swordSprite.flipY = _flipYWhenFacingLeft && dir.x < -0.5f;
    }

    // Snap the player's facing to the nearest of the 4 cardinal directions.
    private Vector2 GetFacing()
    {
        Vector2 facing = _playerMovement != null ? _playerMovement.FacingDirection : Vector2.down;
        if (facing.sqrMagnitude < 0.0001f) facing = Vector2.down;

        if (Mathf.Abs(facing.x) >= Mathf.Abs(facing.y))
            return new Vector2(Mathf.Sign(facing.x), 0f);
        return new Vector2(0f, Mathf.Sign(facing.y));
    }
}
