using UnityEngine;

public class CompanionRangeManager : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _upRange;
    [SerializeField] private BoxCollider2D _downRange;
    [SerializeField] private BoxCollider2D _rightRange;
    [SerializeField] private BoxCollider2D _leftRange;

    public BoxCollider2D ActiveRange { get; private set; }

    void Start()
    {
        _upRange.enabled = false;
        _downRange.enabled = false;
        _rightRange.enabled = false;
        _leftRange.enabled = true;
        ActiveRange = _leftRange;
    }

    public void UpEnable()
    {
        _upRange.enabled = true;
        ActiveRange = _upRange;
    }

    public void UpDisable()
    {
        _upRange.enabled = false;
        if (ActiveRange == _upRange) ActiveRange = null;
    }

    public void DownEnable()
    {
        _downRange.enabled = true;
        ActiveRange = _downRange;
    }

    public void DownDisable()
    {
        _downRange.enabled = false;
        if (ActiveRange == _downRange) ActiveRange = null;
    }

    public void RightEnable()
    {
        _rightRange.enabled = true;
        ActiveRange = _rightRange;
    }

    public void RightDisable()
    {
        _rightRange.enabled = false;
        if (ActiveRange == _rightRange) ActiveRange = null;
    }

    public void LeftEnable()
    {
        _leftRange.enabled = true;
        ActiveRange = _leftRange;
    }

    public void LeftDisable()
    {
        _leftRange.enabled = false;
        if (ActiveRange == _leftRange) ActiveRange = null;
    }
}
