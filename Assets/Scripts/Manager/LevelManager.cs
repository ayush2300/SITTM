using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cardSpawner;  // Assign your CardSpawner GameObject in Inspector
    public float timer;

    public float SceneChangeTimer;

    private void Update()
    {
        timer -= Time.deltaTime;
        SceneChangeTimer -= Time.deltaTime;
        if (timer < 0)
        {
            cardSpawner.SetActive(true);
            timer = 999;
        }

        if (SceneChangeTimer < 0)
        {
            SceneManager.LoadScene("cutScene");
        }


    }
}
