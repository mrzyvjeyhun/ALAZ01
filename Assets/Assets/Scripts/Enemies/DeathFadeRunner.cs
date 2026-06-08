using System.Collections;
using UnityEngine;

public class DeathFadeRunner : MonoBehaviour
{
    private bool _running;

    public static void Kill(GameObject target, float fadeDuration)
    {
        if (target == null) return;

        DeathFadeRunner runner = target.GetComponent<DeathFadeRunner>();
        if (runner == null) runner = target.AddComponent<DeathFadeRunner>();
        runner.Begin(fadeDuration);
    }

    private void Begin(float fadeDuration)
    {
        if (_running) return;
        _running = true;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>()) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        EnemyAI enemy = GetComponent<EnemyAI>();
        if (enemy != null) enemy.enabled = false;

        incibusAI incubus = GetComponent<incibusAI>();
        if (incubus != null) incubus.enabled = false;

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null) anim.enabled = false;

        if (fadeDuration <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(FadeRoutine(fadeDuration));
    }

    private IEnumerator FadeRoutine(float fadeDuration)
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        float[] startAlpha = new float[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
            startAlpha[i] = sprites[i].color.a;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);

            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] == null) continue;
                Color c = sprites[i].color;
                c.a = Mathf.Lerp(startAlpha[i], 0f, k);
                sprites[i].color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
