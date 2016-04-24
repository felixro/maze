using UnityEngine;
using System.Collections;

public class MazeWall : MazeCellEdge 
{
    public Transform wall;

    public override void Initialize(MazeCell cell, MazeCell other, MazeDirection direction)
    {
        base.Initialize(cell, other, direction);

        wall.GetComponent<Renderer>().material = cell.room.mazeRoomSettings.wallMaterial;
    }
}
