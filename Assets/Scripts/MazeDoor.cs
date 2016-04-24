﻿using UnityEngine;
using System.Collections;

public class MazeDoor : MazePassage 
{
    public Transform hinge;

    private MazeDoor OtherSideOfDoor 
    {
        get 
        {
            return other.getEdge(direction.GetOpposite()) as MazeDoor;
        }
    }

    public override void Initialize (MazeCell primary, MazeCell other, MazeDirection direction) 
    {
        base.Initialize(primary, other, direction);
        if (OtherSideOfDoor != null) 
        {
            hinge.localScale = new Vector3(-1f, 1f, 1f);
            Vector3 p = hinge.localPosition;
            p.x = -p.x;
            hinge.localPosition = p;
        }
    }
}
