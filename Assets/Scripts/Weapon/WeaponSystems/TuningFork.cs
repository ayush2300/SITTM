using System.Collections;
using UnityEngine;

public class TuningFork : MonoBehaviour
{
    [Header("Particle Settings")]
    public ParticleSystem particleEffect;

    [Header("Life Cycle Settings")]
    public float lifeTime = 3f;       // total life duration
    public float pauseStart = 1f;     // pause starts at 1s
    public float pauseEnd = 2f;       // pause ends at 2s

    [Header("Freeze time")]
    public float freezeTime = 1f;
    private Coroutine lifeRoutine;

    void OnEnable()
    {
        // Reset and start the particle life cycle
        if (particleEffect != null)
        {
            particleEffect.Clear();
            particleEffect.Play();
        }

        lifeRoutine = StartCoroutine(LifeCycle());
    }

    void OnDisable()
    {
        // Stop and reset everything
        if (lifeRoutine != null)
        {
            StopCoroutine(lifeRoutine);
            lifeRoutine = null;
        }

        if (particleEffect != null)
        {
            particleEffect.Stop();
            particleEffect.Clear();
        }
    }

    IEnumerator LifeCycle()
    {
        float elapsed = 0f;

        while (elapsed < lifeTime)
        {
            // 0 - pauseStart: play
            if (elapsed < pauseStart)
            {
                if (particleEffect != null && !particleEffect.isPlaying)
                    particleEffect.Play();
            }
            // pauseStart - pauseEnd: pause
            else if (elapsed >= pauseStart && elapsed < pauseEnd)
            {
                if (particleEffect != null && particleEffect.isPlaying)
                    particleEffect.Pause();
            }
            // pauseEnd - lifeTime: play
            else
            {
                if (particleEffect != null && !particleEffect.isPlaying)
                    particleEffect.Play();
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Stop at the end of lifeTime
        if (particleEffect != null)
            particleEffect.Stop();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyAI>().Freez(freezeTime);
        }
    }
}
