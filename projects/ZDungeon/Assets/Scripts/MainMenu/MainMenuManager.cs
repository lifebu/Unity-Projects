using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        SoundManager.instance.Play("Fairy Fountain", SoundManager.SoundType.Music, true);
	}

    public void StartPressed()
    {
        SceneManager.LoadScene("Dungeon");
    }

    public void QuitPressed()
    {
        Application.Quit();
    }
}
