using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGameControl : MonoBehaviour
{

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
        //winPanel.SetActive(true);
        Debug.Log("Level Won!");
	}
    public void checkIfWon()
	{
        foreach(GameObject character in characters)
		{
			if (!character.GetComponent<Character>().getFinished())
			{
                return;
			}
		}
        levelWon();
	}

}
