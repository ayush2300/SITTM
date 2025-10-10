using UnityEngine;
using UnityEngine.AI;

public enum ParticleType
{
    Proton,
    Neutron,
    Electron
}

public class ParticleEnemy : MonoBehaviour
{
    public ParticleType type;
    public float moveSpeed = 3f;
    public int collisionDamage = 10;
    public int expDrop = 10;
    public GameObject atomPrefab;
    public float formationRadius = 1f;

    private bool isDead = false;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = moveSpeed;
        }
    }

    private void Update()
    {
        if (isDead) return;

        var target = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (target != null && agent != null)
            agent.SetDestination(target.position);

        TryFormAtom(); // Check each frame if atom can be formed
    }

    private void TryFormAtom()
    {
        Collider2D[] nearbyParticles = Physics2D.OverlapCircleAll(transform.position, formationRadius);
        bool hasProton = false, hasNeutron = false, hasElectron = false;

        foreach (var collider in nearbyParticles)
        {
            ParticleEnemy p = collider.GetComponent<ParticleEnemy>();
            if (p != null)
            {
                
                switch (p.type)
                {
                    case ParticleType.Proton:
                        hasProton = true;
                        break;
                    case ParticleType.Neutron:
                        hasNeutron = true;
                        break;
                    case ParticleType.Electron:
                        hasElectron = true;
                        break;
                }
            }
        }

        if (hasProton && hasNeutron && hasElectron)
        {
            Debug.Log("Atom formation conditions met. Forming atom.");
            SpawnAtom();

            foreach (var collider in nearbyParticles)
            {
                ParticleEnemy p = collider.GetComponent<ParticleEnemy>();
                if (p != null &&
                    (p.type == ParticleType.Proton || p.type == ParticleType.Neutron || p.type == ParticleType.Electron))
                {
                    p.Die();
                }
            }
        }
    }

    private void SpawnAtom()
    {
        Instantiate(atomPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        ParticleEnemy otherParticle = other.GetComponent<ParticleEnemy>();
        if (otherParticle != null && otherParticle.type != this.type)
        {
            Debug.Log($"Trigger entered with different particle: {otherParticle.type}");
            // Optional: Could try form atom here as well
        }
    }

    public void Die()
    {
        isDead = true;
        gameObject.SetActive(false);
    }
}
