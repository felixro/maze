using UnityEngine;
using System.Collections;

public abstract class MazeCellEdge : MonoBehaviour 
{
    public MazeCell cell;
    public MazeCell other;
    public MazeDirection direction;

    public virtual void Initialize(MazeCell cell, MazeCell other, MazeDirection direction)
    {
        this.cell = cell;
        this.other = other;
        this.direction = direction;
        cell.SetEdge(direction, this);
        transform.parent = cell.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = direction.ToRotation();
    }
}
