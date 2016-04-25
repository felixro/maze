using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Maze maze;
    public PlayerController player;
    public Camera mapCamera;

    private Maze mazeInstance;

	private void Start () 
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
        mapCamera.rect = new Rect(0f, 0f, 0.5f, 0.5f);
        mapCamera.clearFlags = CameraClearFlags.Skybox;
        mapCamera.clearFlags = CameraClearFlags.Depth;
        mazeInstance = Instantiate(maze) as Maze;
        mazeInstance.Generate(player);
    }

    private void RestartGame()
    {
        StopAllCoroutines();
        Destroy(mazeInstance.gameObject);
        BeginGame();
    }
}
