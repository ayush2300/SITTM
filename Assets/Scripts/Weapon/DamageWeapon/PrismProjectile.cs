using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(TrailRenderer))]
public class PrismProjectile : MonoBehaviour
{
    private Vector2 direction;
    private Vector2 initialDirection;
    private float speed;
    private float targetBendAngle; // total bend angle (degrees)
    public float curveDuration = 0.5f; // how long it takes to complete the bend
    public int damage = 10;

    public float lifeTime = 3f;
    public float curveDelay = 0.3f; // time before bending starts

    private float timeAlive = 0f;
    private float bendProgress = 0f;
    private bool bending = false;

    private SpriteRenderer sr;
    private TrailRenderer trail;
    private Color lastTrailColor = Color.clear;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();

        // Ensure the TrailRenderer has its own material that supports vertex colors
        if (trail != null)
        {
            Shader spriteShader = Shader.Find("Sprites/Default");
            if (spriteShader != null)
                trail.material = new Material(spriteShader);
            else
                trail.material = new Material(Shader.Find("Sprites/Default")); // try anyway
        }
    }

    void Update()
    {
        timeAlive += Time.deltaTime;

        // Start bending after the delay
        if (!bending && timeAlive >= curveDelay)
        {
            bending = true;
            bendProgress = 0f; // reset progress when starting
        }

        // Update bend progress and rotate the initial direction smoothly
        if (bending && bendProgress < 1f)
        {
            bendProgress += Time.deltaTime / Mathf.Max(0.0001f, curveDuration);
            float currentAngle = Mathf.Lerp(0f, targetBendAngle, bendProgress);
            direction = Quaternion.Euler(0f, 0f, currentAngle) * initialDirection;
        }

        // If not bending yet, keep moving along initial direction
        if (!bending)
        {
            direction = initialDirection;
        }

        // Move forward
        transform.localPosition += (Vector3)(direction.normalized * speed * Time.deltaTime);

        // Rotate sprite to match direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Update trail color (only when sprite color changed)
        if (sr != null && trail != null)
        {
            Color projColor = sr.color;
            projColor.a = 1f; // ensure full alpha

            if (!ColorsEqual(projColor, lastTrailColor))
            {
                Gradient g = new Gradient();
                g.SetKeys(
                    new GradientColorKey[] {
                        new GradientColorKey(projColor, 0f),
                        new GradientColorKey(projColor, 1f)
                    },
                    new GradientAlphaKey[] {
                        new GradientAlphaKey(1f, 0f),
                        new GradientAlphaKey(1f, 1f)
                    }
                );
                trail.colorGradient = g;
                lastTrailColor = projColor;
            }
        }

        // Destroy after lifetime
        if (timeAlive >= lifeTime)
            Destroy(gameObject);
    }

    // Use this to launch. 'bend' is the total extra rotation (degrees) to apply over curveDuration.
    public void Launch(Vector2 dir, float spd, float bend)
    {
        initialDirection = dir.normalized;
        direction = initialDirection;
        speed = spd;
        targetBendAngle = bend;
        timeAlive = 0f;
        bendProgress = 0f;
        bending = false;
    }

    // small helper to compare colors with tolerance
    private bool ColorsEqual(Color a, Color b, float eps = 0.001f)
    {
        return Mathf.Abs(a.r - b.r) < eps &&
               Mathf.Abs(a.g - b.g) < eps &&
               Mathf.Abs(a.b - b.b) < eps &&
               Mathf.Abs(a.a - b.a) < eps;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                enemy.Damage(damage);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                enemy.Damage(damage);
            }
        }
    }
}
