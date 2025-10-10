using UnityEngine;
using DG.Tweening;

public class InteractiveFallingItem : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefabToSpawn;

    [Header("Chance Settings")]
    public float fallChance = 0.2f;
    public float chanceMultiplier = 0.05f;

    [Header("Detection Settings")]
    public float detectionRadius = 1.5f;

    [Header("Fall & Animation")]
    public Transform fallTarget;
    public float fallDuration = 0.7f;
    public float arcHeight = 2f;
    public float popScale = 1.2f;
    public float popDuration = 0.15f;

    [Header("Rotation Settings")]
    public Vector3 targetRotationEuler = Vector3.zero;

    [Header("Damage Settings")]
    public int damage = 10;

    [Header("Destroy")]
    public bool destroyOnFall = true;

    private bool hasFallen = false;
    private Vector3 originalScale;
    private Quaternion originalRotation;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (fallTarget != null)
        {
            Vector3 start = transform.position;
            Vector3 end = fallTarget.position;
            Gizmos.color = Color.yellow;
            int segs = 16;
            Vector3 prev = start;
            for (int i = 1; i <= segs; ++i)
            {
                float t = i / (float)segs;
                Vector3 pos = Parabola(start, end, arcHeight, t);
                Gizmos.DrawLine(prev, pos);
                prev = pos;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(end, 0.3f);
        }
    }

    void Update()
    {
        if (!hasFallen)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            foreach (var col in hits)
            {
                if (col.CompareTag("Player"))
                {
                    fallChance += chanceMultiplier;
                    if (Random.value < fallChance)
                    {
                        StartPopAndFall();
                    }
                    break;
                }
            }
        }
    }

    void StartPopAndFall()
    {
        hasFallen = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(popScale * originalScale, popDuration).SetEase(Ease.OutBack));
        seq.Append(transform.DOScale(originalScale, popDuration).SetEase(Ease.InQuart));
        seq.AppendCallback(DoArcFall);
        seq.SetUpdate(true);
    }

    void DoArcFall()
    {
        if (fallTarget == null) return;
        Vector3 start = transform.position;
        Vector3 end = fallTarget.position;
        float tVal = 0;

        Tweener moveTween = DOTween.To(() => tVal, x =>
        {
            tVal = x;
            transform.position = Parabola(start, end, arcHeight, tVal);
        }, 1f, fallDuration).SetEase(Ease.InQuad);

        Tweener rotateTween = transform.DOLocalRotate(targetRotationEuler, fallDuration).SetEase(Ease.Linear);

        Sequence fallSeq = DOTween.Sequence();
        fallSeq.Join(moveTween);
        fallSeq.Join(rotateTween);
        fallSeq.OnComplete(() =>
        {
            TryHitPlayer();

            if (prefabToSpawn != null && fallTarget != null)
            {
                Instantiate(prefabToSpawn, fallTarget.position, Quaternion.identity);
            }

            if (destroyOnFall) Destroy(gameObject);
        });
    }

    void TryHitPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(fallTarget.position, 0.4f);
        foreach (var col in hits)
        {
            if (col.CompareTag("Player"))
            {
                var healthSys = col.GetComponent<HealthSystem>();
                if (healthSys != null) healthSys.Damage(damage);
            }
        }
    }

    Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 peak = (start + end) / 2 + Vector3.up * height;
        Vector3 a = Vector3.Lerp(start, peak, t);
        Vector3 b = Vector3.Lerp(peak, end, t);
        return Vector3.Lerp(a, b, t);
    }
}
