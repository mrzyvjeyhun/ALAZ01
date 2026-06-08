using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireDamage : MonoBehaviour
{
    [SerializeField] public float _health;
    [SerializeField] public float _maxHealth = 10f;
    [SerializeField] public float _incubusDamage = 5f;
    [SerializeField] private Button _restartButton;
    [SerializeField] private FireLight _fireLight;
    [SerializeField] private FireDamageAudio _damageAudio;
    [SerializeField] private float _deathFadeDuration = 0.4f;
    [HideInInspector] public bool _isDead;
    void Awake()
    {
        _health = _maxHealth;

        if (_fireLight == null) _fireLight = FindObjectOfType<FireLight>();
        if (_damageAudio == null) _damageAudio = GetComponent<FireDamageAudio>();

        _restartButton.gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (_damageAudio != null) _damageAudio.PlayDamageSFX();

        if (_health <= 0)
        Die();
    }

    public void Die()
    {
        _isDead = true;

        _restartButton.gameObject.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("girdi");
            TakeDamage(1);
            DeathFadeRunner.Kill(collision.gameObject, _deathFadeDuration);
            FlashRed();
        }

        if (collision.gameObject.CompareTag("Incubus"))
        {
            DeathFadeRunner.Kill(collision.gameObject, _deathFadeDuration);
            TakeDamage(_incubusDamage);
            FlashRed();
        }
    }

    private void FlashRed()
    {
        if (_fireLight != null) _fireLight.Flash(Color.red);
    }
}
