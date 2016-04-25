using UnityEngine;
using System.Collections;

public class MazeDoor : MazePassage 
{
    public Transform hinge;

    private bool opened;

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

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != hinge)
            {
                child.GetComponent<Renderer>().material = cell.room.mazeRoomSettings.wallMaterial;
            }
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (!opened)
        {
            Vector3 curPosition = transform.localPosition;
            Vector3 position1 = new Vector3(curPosition.x+0.5f, curPosition.y, curPosition.z+0.5f);
            transform.localPosition = position1;

            curPosition = OtherSideOfDoor.transform.localPosition;
            Vector3 position2 = new Vector3(curPosition.x, curPosition.y, curPosition.z);
            OtherSideOfDoor.transform.localPosition = position2;

            OtherSideOfDoor.hinge.localRotation = hinge.localRotation = Quaternion.Euler(0f, -90f, 0f);

            opened = true;
        }
    }
}
