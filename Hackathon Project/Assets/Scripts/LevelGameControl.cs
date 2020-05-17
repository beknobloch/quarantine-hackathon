using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelGameControl : MonoBehaviour
{

    public bool currentlyDrawing = false;

    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private GameObject losePanel;
    [SerializeField]
    private List <GameObject> characters = new List<GameObject>();

    public void levelLost()
	{
        losePanel.SetActive(true);
	}
    public void levelWon()
	{
        winPanel.SetActive(true);
        Debug.Log("Level Won!");
	}
    public void checkIfWon()
	{
        int numEnabled = 0;
        foreach(GameObject character in characters)
		{
			if (character.activeSelf)
			{
                numEnabled += 1;
			}
		}
        if(numEnabled < 1) {
            levelWon();
        }
	}
    public void changeScene(string sceneName)
	{
        SceneManager.LoadScene(sceneName);
    }
}
