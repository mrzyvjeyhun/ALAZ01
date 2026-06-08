using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float _speed = 12f;

    [SerializeField] private float _lifeTime = 5f;
    
    [SerializeField] private float _spriteAngleOffset = 0f;
    [SerializeField] private float _animTime = 0.3f;

    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _sr;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void Launch(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;
        direction = direction.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + _spriteAngleOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        _rb.linearVelocity = direction * _speed;

        Destroy(gameObject, _lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Break();
        }
    }

    void Break()
    {
        StartCoroutine(BreakCor());
    }

    private IEnumerator BreakCor()
    {
        _rb.linearVelocity = Vector2.zero;
        _anim.SetTrigger("Break");

        float startAlpha = _sr != null ? _sr.color.a : 1f;
        float t = 0f;
        while (t < _animTime)
        {
            t += Time.deltaTime;

            if (_sr != null)
            {
                Color c = _sr.color;
                c.a = Mathf.Lerp(startAlpha, 0f, t / _animTime);
                _sr.color = c;
            }

            yield return null;
        }

        GameObject.Destroy(gameObject);
    }

    private GameObject GetEnemyRoot(Collider2D col)
    {
        EnemyAI enemy = col.GetComponentInParent<EnemyAI>();
        if (enemy != null) return enemy.gameObject;

        incibusAI incubus = col.GetComponentInParent<incibusAI>();
        if (incubus != null) return incubus.gameObject;

        if (col.CompareTag("Enemy") || col.CompareTag("Incubus")) return col.gameObject;

        return null;
    }
}
