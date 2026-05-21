using UnityEngine;

public enum MazeObjectType { Wall, Obstacle, Exit }

public class MarkerComponent : MonoBehaviour
{
    public MazeObjectType objectType;
}
