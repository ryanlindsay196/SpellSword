using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadHarbingerScene : MonoBehaviour
{
    public int harbingerScene;
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<CharacterMovement>() != null)
        {
            gameObject.GetComponent<AudioSource>().Play();
            SceneManager.LoadScene(harbingerScene);
        }
    }
}
