using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Maze maze;
    public PlayerController player;

    private Maze mazeInstance;

	private void Start () 
    {
        StartCoroutine(BeginGame());
	}
	
	void Update () 
    {
        if ( Input.GetKeyDown( KeyCode.R ) )
        {
            RestartGame();
        }
	}

    private IEnumerator BeginGame()
    {
        mazeInstance = Instantiate(maze) as Maze;
        yield return StartCoroutine(mazeInstance.Generate(player));
    }

    private void RestartGame()
    {
        StopAllCoroutines();
        Destroy(mazeInstance.gameObject);
        BeginGame();
    }
}
