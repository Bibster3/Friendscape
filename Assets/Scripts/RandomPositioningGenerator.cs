using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RandomPositioningGenerator : MonoBehaviour
{
    [SerializeField] private Button shuffleButton;
    public static event EventHandler OnRandomPuzzleButtonCLicked;
    private List<Vector3Int> validPositions;
    public void RandomlyPositionMovableObjects()
    {
        GameOverUI.Instance.HideGameOver();
        GameManager.isGameOver = false;
        GameManager.Instance.ResetTimer();
        shuffleButton.gameObject.SetActive(true);
        OnRandomPuzzleButtonCLicked?.Invoke(this, EventArgs.Empty);

        // List of movable objects and their corresponding prefabs
        List<(MovableObject, GameObject)> movableObjectPrefabs = new()
        {
            (GameManager.Instance.playerObject, GameManager.Instance.playerPrefab),
            (GameManager.Instance.boxesObject, GameManager.Instance.boxesPrefab),
            (GameManager.Instance.suitcasesObject, GameManager.Instance.suitcasesPrefab),
            (GameManager.Instance.bikeObject, GameManager.Instance.bikePrefab),
            (GameManager.Instance.deskObject, GameManager.Instance.deskPrefab),
        };

        Vector3Int initPlayerPosition = new Vector3Int(0, 4, 0);
        GameManager.Instance.playerObject = Instantiate(GameManager.Instance.playerPrefab, GameManager.Instance.tilemap.GetCellCenterWorld(initPlayerPosition), Quaternion.identity, GameManager.Instance.tilemap.transform).GetComponent<MovableObject>();
        GameManager.Instance.MarkOccupiedCells(initPlayerPosition, GameManager.Instance.playerObject.Size);

        validPositions = GenerateAllValidPositions();

        foreach (var (movableObject, prefab) in movableObjectPrefabs)
        {
            if (movableObject is Player)
            {
                continue;
            }

            Vector3Int randomPosition;
            randomPosition = FindValidPositionForMovableObject(movableObject.Size);
            GameObject instantiatedObject = Instantiate(prefab, GameManager.Instance.tilemap.GetCellCenterWorld(randomPosition), Quaternion.identity, GameManager.Instance.tilemap.transform);
            MovableObject instantiatedMovableObject = instantiatedObject.GetComponent<MovableObject>();
            instantiatedMovableObject.position = randomPosition;
            instantiatedMovableObject.SetGameObject(instantiatedObject);             // Set the GameObject of the MovableObject to the instantiated prefab
            GameManager.Instance.MarkOccupiedCells(randomPosition, movableObject.Size);
            GameManager.Instance.movableObjects.Add(instantiatedMovableObject);
        }
    }

    private List<Vector3Int> GenerateAllValidPositions()
    {
        List<Vector3Int> validPositions = new List<Vector3Int>();

        for (int x = 0; x < Constants.gridSizeX; x++)
        {
            for (int y = 0; y < Constants.gridSizeY; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                validPositions.Add(position);
            }
        }
        return validPositions;
    }
    private Vector3Int FindValidPositionForMovableObject(Vector2Int size)
    {
        // Shuffle the valid positions list to get a random order for backtracking
        ShuffleValidPositions();

        foreach (Vector3Int position in validPositions)
        {
            if (CanFitAtPosition(position, size))
            {
                return position;
            }
        }
        return Vector3Int.zero; // No valid position found
    }

    private void ShuffleValidPositions()
    {
        int n = validPositions.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            Vector3Int value = validPositions[k];
            validPositions[k] = validPositions[n];
            validPositions[n] = value;
        }
    }
    private bool CanFitAtPosition(Vector3Int position, Vector2Int size)
    {
        for (int xOffset = 0; xOffset < size.x; xOffset++)
        {
            for (int yOffset = 0; yOffset < size.y; yOffset++)
            {
                Vector3Int cell = new Vector3Int(position.x + xOffset, position.y + yOffset, position.z);

                var isCellWithinBounds = GameManager.Instance.IsCellWithinBounds(cell);
                var isCellOccupied = GameManager.Instance.IsCellOccupied(cell);
                if (!isCellWithinBounds || isCellOccupied)
                {
                    return false;
                }
            }
        }
        return true;
    }
}