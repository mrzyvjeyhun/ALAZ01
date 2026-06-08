using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float _health = 2f;
    [SerializeField] private float _deathFadeDuration = 0.4f;

    private BoxCollider2D _bx;

    void Awake()
    {
        _bx = GetComponent<BoxCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Arrow")){
            Debug.Log("ok");
            TakeDamage(1);
        }
    }

    public void TakeDamage(float damage)
    {
        --_health;

        if (_health <= 0) Die();
    }

    private void Die()
    {
        DeathFadeRunner.Kill(gameObject, _deathFadeDuration);
    }
}
