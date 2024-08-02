using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant1: NonMovableObject
{
    public override Vector2Int Size => new Vector2Int(2, 3);

    public override Vector3Int Position => new Vector3Int(4, 3, 0);
}