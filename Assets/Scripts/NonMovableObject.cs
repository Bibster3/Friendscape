using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NonMovableObject : MonoBehaviour
{
    public abstract Vector2Int Size { get; }

    public abstract Vector3Int Position { get; }
}
