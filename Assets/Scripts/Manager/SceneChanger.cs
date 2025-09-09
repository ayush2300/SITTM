using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string SceneName;
    public float timerTOChange;

    private void Update()
    {
        timerTOChange -= Time.deltaTime;
        if(timerTOChange < 0 )
            SceneManager.LoadScene( SceneName );
    }
}
