using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [Header("Solar System Settings")]
    public List<GameObject> planetPrefabs; // Assign different planet prefabs in Inspector
    public float minOrbitRadius = 2f;      // Radius of first planet
    public float maxOrbitRadius = 4f;      // Radius of last planet
    public float minOrbitSpeed = 30f;
    public float maxOrbitSpeed = 100f;
    public float damage = 10f;

    private Transform player;
    private List<PlanetData> planets = new List<PlanetData>();

    [System.Serializable]
    private class PlanetData
    {
        public GameObject planet;
        public float angle;
        public float speed;
        public float radius; // Unique radius for this planet
    }

    void Start()
    {
        player = transform.parent; // Weapon is attached to Player

        if (planetPrefabs.Count == 0)
        {
            Debug.LogWarning("No planet prefabs assigned to SolarSystem!");
            return;
        }

        int numberOfPlanets = planetPrefabs.Count;
        float radiusStep = (numberOfPlanets == 1) ? 0f : (maxOrbitRadius - minOrbitRadius) / (numberOfPlanets - 1);

        for (int i = 0; i < numberOfPlanets; i++)
        {
            float angle = (360f / numberOfPlanets) * i;
            float radius = minOrbitRadius + radiusStep * i;

            Vector3 position = player.position + Quaternion.Euler(0, 0, angle) * Vector3.right * radius;
            GameObject planet = Instantiate(planetPrefabs[i], position, Quaternion.identity, transform);

            PlanetData data = new PlanetData
            {
                planet = planet,
                angle = angle,
                speed = Random.Range(minOrbitSpeed, maxOrbitSpeed),
                radius = radius
            };

            planets.Add(data);
        }
    }

    void Update()
    {
        if (player == null) return;

        // Update positions of each planet individually
        foreach (var planetData in planets)
        {
            planetData.angle += planetData.speed * Time.deltaTime;
            if (planetData.angle > 360f) planetData.angle -= 360f;

            Vector3 offset = Quaternion.Euler(0, 0, planetData.angle) * Vector3.right * planetData.radius;
            planetData.planet.transform.position = player.position + offset;
        }
    }
}
