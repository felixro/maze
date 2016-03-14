using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Maze maze;
    public PlayerController player;

    private Maze mazeInstance;

	void Start () 
    {
        BeginGame();
	}
	
	void Update () 
    {
        if ( Input.GetKeyDown( KeyCode.R ) )
        {
            RestartGame();
        }
	}

    private void BeginGame()
    {
        mazeInstance = Instantiate(maze) as Maze;
        //mazeInstance.Generate();
        StartCoroutine(mazeInstance.Generate(player));
    }

    private void RestartGame()
    {
        StopAllCoroutines();
        Destroy(mazeInstance.gameObject);
        BeginGame();
    }
}
