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
    public MazeDoor doorPrefab;
    public MazeRoomSettings[] mazeRoomSettings;

    [Range(0f, 1f)]
    public float doorProbability;
   
    MazeCell[,] cells;

    public float generationStepDelay;

    private PlayerController playerInstance;

    private List<MazeRoom> rooms = new List<MazeRoom>();

    private MazeRoom createRoom(int indexToExclude)
    {
        MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
        newRoom.settingsIndex = Random.Range(0, mazeRoomSettings.Length);
        if (newRoom.settingsIndex == indexToExclude)
        {
            newRoom.settingsIndex = (newRoom.settingsIndex + 1) % mazeRoomSettings.Length;
        }
        newRoom.mazeRoomSettings = mazeRoomSettings[newRoom.settingsIndex];
        rooms.Add(newRoom);
        return newRoom;
    }

    public MazeCell GetCell( IntVector2 coordinates )
    {
        return cells[coordinates.x, coordinates.z];
    }

    public void Generate (PlayerController player) 
    {
        cells = new MazeCell[size.x, size.z];

        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells, player);
        while ( activeCells.Count > 0 )
        {
            DoNextGenerationStep(activeCells);
        }
    }

    private void DoFirstGenerationStep(List<MazeCell> activeCells, PlayerController player)
    {
        IntVector2 coordinates = RandomCoordinates;
        MazeCell newCell = CreateCell(coordinates);
        newCell.Initialize(createRoom(-1));

        activeCells.Add(newCell);

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
            }
            else if (currentCell.room.mazeRoomSettings == neighbour.room.mazeRoomSettings)
            {
                CreatePassageInSameRoom(currentCell, neighbour, direction);
            }
            else
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

    private void CreatePassageInSameRoom(MazeCell from, MazeCell to, MazeDirection direction)
    {
        MazePassage passage = Instantiate(mazePassage) as MazePassage;
        passage.Initialize(from, to, direction);
        passage = Instantiate(mazePassage) as MazePassage;
        passage.Initialize(to, from, direction.GetOpposite());

        if (from.room != to.room)
        {
            MazeRoom roomToAssimilate = to.room;
            from.room.Assimilate(roomToAssimilate);
            rooms.Remove(roomToAssimilate);
            Destroy(roomToAssimilate);
        }
    }

    private void CreatePassage(MazeCell from, MazeCell to, MazeDirection direction)
    {
        MazePassage prefab = Random.value < doorProbability ? doorPrefab : mazePassage;
        MazePassage passage = Instantiate(prefab) as MazePassage;

        if (passage is MazeDoor)
        {
            to.Initialize(createRoom(from.room.settingsIndex));
        }
        else
        {
            to.Initialize(from.room);
        }

        passage.Initialize(from, to, direction);
        passage = Instantiate(prefab) as MazePassage;
        passage.Initialize(to, from, direction.GetOpposite());
    }
        
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
