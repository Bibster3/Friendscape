using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant2 : NonMovableObject
{
    // Size of the plant (2x2 tiles in this case)
    public override Vector2Int Size => new Vector2Int(2, 2);

    public override Vector3Int Position => new Vector3Int(0, 0, 0);

}