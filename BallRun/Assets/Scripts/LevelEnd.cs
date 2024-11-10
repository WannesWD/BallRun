using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{ 
    private void OnTriggerEnter(Collider other)
    {
        //load next level if player enters trigger
        PlayerCharacter player = other.gameObject.GetComponent<PlayerCharacter>();
        if(player)
        {
            int currentSceneidx = SceneManager.GetActiveScene().buildIndex;

            SceneManager.LoadScene(++currentSceneidx % SceneManager.sceneCountInBuildSettings);
        }
    }
}
