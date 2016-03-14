using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour 
{
    public MazeCell mazeCell;
    public IntVector2 size;
    public MazePassage mazePassage;
    public MazeWall[] wallPrefabs;
    public MazeType type;

    private PlayerController playerInstance;

    MazeCell[,] cells;

    public float generationStepDelay;

    public MazeCell GetCell( IntVector2 coordinates )
    {
        return cells[coordinates.x, coordinates.z];
    }

    public IEnumerator Generate (PlayerController player) 
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        cells = new MazeCell[size.x, size.z];

        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells, player);
        while ( activeCells.Count > 0 )
        {
            yield return delay;
            DoNextGenerationStep(activeCells);
        }
    }

    private void DoFirstGenerationStep( List<MazeCell> activeCells, PlayerController player )
    {
        IntVector2 coordinates = RandomCoordinates;
        activeCells.Add(CreateCell(coordinates));
        PlayerController playerInstance = Instantiate(player) as PlayerController;
        playerInstance.transform.parent = transform;
        playerInstance.transform.localPosition = 
            new Vector3(
                coordinates.x - size.x * 0.5f + 0.5f, 
                1f, 
                coordinates.z - size.z * 0.5f + 0.5f
            );
    }

    private void DoNextGenerationStep( List<MazeCell> activeCells )
    {
        int currentIndex;
        switch( type )
        {
            case MazeType.FIRST:
                currentIndex = 0;
                break;
            case MazeType.LAST:
                currentIndex = activeCells.Count - 1;
                break;
            case MazeType.RANDOM:
                currentIndex = Random.Range(0,activeCells.Count - 1);
                break;
            default:
                currentIndex = 0;
                break;
        }
        //int currentIndex = activeCells.Count - 1;
        //int currentIndex = Random.Range(0,activeCells.Count - 1);

        MazeCell currentCell = activeCells[currentIndex];
        if ( currentCell.IsFullyInitialized )
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }

        MazeDirection direction = currentCell.RandomUninitializedDirection;
        IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
        if ( ContainsCoordinates(coordinates) )
        {
            MazeCell neighbour = GetCell(coordinates);
            if ( neighbour == null )
            {
                neighbour = CreateCell(coordinates);
                CreatePassage(currentCell, neighbour, direction);
                activeCells.Add(neighbour);
            }else
            {
                CreateWall(currentCell, neighbour, direction);
            }
        }else
        {
            // outside of maze
            CreateWall(currentCell, null, direction);
        }
    }

    private void CreateWall(MazeCell from, MazeCell to, MazeDirection direction)
    {
        MazeWall wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
        wall.Initialize(from, to, direction);
        if ( to != null )
        {
            wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
            wall.Initialize(to, from, direction.GetOpposite());
        }
    }

    private void CreatePassage(MazeCell from, MazeCell to, MazeDirection direction)
    {
        MazePassage passage = Instantiate(mazePassage) as MazePassage;
        passage.Initialize(from, to, direction);
        passage = Instantiate(mazePassage) as MazePassage;
        passage.Initialize(to, from, direction.GetOpposite());
    }

    /*
    public void Generate()
    {
        cells = new MazeCell[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++) 
        {
            for (int y = 0; y < sizeY; y++) 
            {
                CreateCell(x,y);
            }
        }
    }
    */
        
    MazeCell CreateCell(IntVector2 coordinates)
    {
        MazeCell newCell = Instantiate(mazeCell) as MazeCell;
        cells[coordinates.x,coordinates.z] = newCell;
        newCell.coordinates = coordinates;
        newCell.name = string.Format("Maze Cell {0}/{1}", coordinates.x, coordinates.z);
        newCell.transform.parent = transform;
        newCell.transform.localPosition = 
            new Vector3(
                coordinates.x - size.x * 0.5f + 0.5f, 
                0f, 
                coordinates.z - size.z * 0.5f + 0.5f
            );
        
        return newCell;
    }

    public IntVector2 RandomCoordinates 
    {
        get 
        {
            return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
        }
    }

    public bool ContainsCoordinates( IntVector2 coordinates )
    {
        return 
            coordinates.x >= 0 &&
            coordinates.x < size.x &&
            coordinates.z >= 0 &&
            coordinates.z < size.z;
    }
}
