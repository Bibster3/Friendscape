using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject playerPrefab, bikePrefab, deskPrefab, boxesPrefab, suitcasesPrefab, plant1Prefab, plant2Prefab;
    public static GameManager Instance;
    public MovableObject selectedObject;
    public MovableObject playerObject, bikeObject, deskObject, boxesObject, suitcasesObject;
    public List<MovableObject> movableObjects = new List<MovableObject>();
    public List<Vector3Int> occupiedPositions = new List<Vector3Int>(); // represents a grid position that is occupied.
    public Vector3Int winningPosition = new Vector3Int(5, 1, 0);
    public static bool isGameOver = false;
    public static event EventHandler OnObjectSelected;
    private NonMovableObject plant1Object, plant2Object;


    
    [Header("Timer")] public AudioSource timerAudioSource;
    
    public AudioClip timerBlinkSound; // 
    public Coroutine timerBlinkCoroutine;
    public GameObject timerHolder;
    private Color originalTimerColor;
    private bool isBlinking;
    public float countdownTime = 30f; // Countdown time in seconds
    private TextMeshProUGUI timerText;
    private int lastSecondTicked = -1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        timerText = timerHolder.GetComponentInChildren<TextMeshProUGUI>(true);
        originalTimerColor = timerText.color;
    }

    void Start()
    {
        InitializeMapOfObjects();
    }

    void Update()
    {
        if (isGameOver == false)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                SelectObject();
            }

            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        timerText.text = countdownTime.ToString("0:00");
        countdownTime -= Time.deltaTime;
        LastSecondsTimerBlink(countdownTime);

        if (countdownTime <= 0)
        {
            isGameOver = true;
            GameOverUI.Instance.ShowGameOver(true);
        }
    }

    private void LastSecondsTimerBlink(float seconds)
    {
        if (seconds <= 5 && seconds > 0 && !isBlinking) 
        {
            isBlinking = true;
            if (timerBlinkCoroutine != null)
            {
                StopCoroutine(timerBlinkCoroutine);
                timerText.color = originalTimerColor;
            }

            timerBlinkCoroutine = StartCoroutine(BlinkTimerCoroutine());
        }

        if (seconds >= 5)
        {
            isBlinking = false;

            if (timerBlinkCoroutine != null)
            {
                StopCoroutine(timerBlinkCoroutine);
                timerBlinkCoroutine = null;
                timerText.color = originalTimerColor; // Reset color
            }
        }

      
       
        int currentSecond = Mathf.CeilToInt(seconds);

        if (currentSecond != lastSecondTicked)
        {
            lastSecondTicked = currentSecond;
            //  if (isBlinking && currentSecond <= 5 && currentSecond > 0) ;
            // Invoke OnTimerBlinkEvent every second during the last 5 seconds
        }
    }

    private IEnumerator BlinkTimerCoroutine()
    {
        while (isBlinking)
        {
            timerText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            timerText.color = originalTimerColor;
            yield return new WaitForSeconds(0.5f);
            if (countdownTime > 0f)
            {
                timerAudioSource.PlayOneShot(timerBlinkSound); 
            }
        }
    }

    public void ResetTimer()
    {
        countdownTime = 30f;
    }

    private void InitializeMapOfObjects()
    {
        Vector3Int initPlant1Position = new Vector3Int(4, 3, 0);
        plant1Object =
            Instantiate(plant1Prefab, tilemap.GetCellCenterWorld(initPlant1Position), Quaternion.identity,
                tilemap.transform).GetComponent<NonMovableObject>();
        MarkOccupiedCells(initPlant1Position, plant1Object.Size);

        Vector3Int initPlant2Position = new Vector3Int(0, 0, 0);
        plant2Object =
            Instantiate(plant2Prefab, tilemap.GetCellCenterWorld(initPlant2Position), Quaternion.identity,
                tilemap.transform).GetComponent<NonMovableObject>();
        MarkOccupiedCells(initPlant2Position, plant2Object.Size);

        Vector3Int initPlayerPosition = new Vector3Int(0, 4, 0);
        playerObject =
            Instantiate(playerPrefab, tilemap.GetCellCenterWorld(initPlayerPosition), Quaternion.identity,
                tilemap.transform).GetComponent<MovableObject>();
        MarkOccupiedCells(initPlayerPosition, playerObject.Size);

        Vector3Int initBikePosition = new Vector3Int(1, 4, 0);
        bikeObject =
            Instantiate(bikePrefab, tilemap.GetCellCenterWorld(initBikePosition), Quaternion.identity,
                tilemap.transform).GetComponent<MovableObject>();
        MarkOccupiedCells(initBikePosition, bikeObject.Size);

        Vector3Int initStrollerPosition = new Vector3Int(1, 2, 0);
        deskObject =
            Instantiate(deskPrefab, tilemap.GetCellCenterWorld(initStrollerPosition), Quaternion.identity,
                tilemap.transform).GetComponent<MovableObject>();
        ;
        MarkOccupiedCells(initStrollerPosition, deskObject.Size);

        Vector3Int initBoxesPosition = new Vector3Int(3, 0, 0);
        boxesObject =
            Instantiate(boxesPrefab, tilemap.GetCellCenterWorld(initBoxesPosition), Quaternion.identity,
                tilemap.transform).GetComponent<MovableObject>();
        MarkOccupiedCells(initBoxesPosition, boxesObject.Size);

        Vector3Int initSuitcasesPosition = new Vector3Int(2, 2, 0);
        suitcasesObject =
            Instantiate(suitcasesPrefab, tilemap.GetCellCenterWorld(initSuitcasesPosition), Quaternion.identity,
                tilemap.transform).GetComponent<MovableObject>();
        MarkOccupiedCells(initSuitcasesPosition, suitcasesObject.Size);

        movableObjects.Add(playerObject);
        movableObjects.Add(bikeObject);
        movableObjects.Add(deskObject);
        movableObjects.Add(boxesObject);
        movableObjects.Add(suitcasesObject);
    }

    private void SelectObject()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = tilemap.WorldToCell(worldMousePos);
        GameObject clickedObject = GetMovableObjectAtPosition(gridPosition);

        if (clickedObject != null)
        {
            MovableObject clickedMovableObject = clickedObject.GetComponent<MovableObject>();
            if (clickedMovableObject != null)
            {
                if (selectedObject != null)
                {
                    selectedObject.isSelected = false;
                }

                selectedObject = clickedMovableObject;
                selectedObject.isSelected = true;
                OnObjectSelected?.Invoke(this, EventArgs.Empty);

                // Hide and stop animation of highlighter if it's the first click
                if (clickedMovableObject.isFirstClick && clickedMovableObject.highlighter != null)
                {
                    clickedMovableObject.highlighterAnimator.enabled = false; // Stop the animation
                    clickedMovableObject.highlighter.SetActive(false);
                    clickedMovableObject.isFirstClick = false;
                }
            }
        }
        else
        {
            if (selectedObject != null)
            {
                selectedObject.isSelected = false;
                selectedObject = null;
                OnObjectSelected?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public bool IsCellOccupied(Vector3Int cell)
    {
        var result = occupiedPositions.Contains(cell); // Check if the position is in the list

        return result;
    }

    public bool IsCellWithinBounds(Vector3Int cell)
    {
        if (cell.x >= 0 && cell.x < Constants.gridSizeX && cell.y >= 0 && cell.y < Constants.gridSizeY)
        {
            return true;
        }

        return false;
    }

    public void MarkOccupiedCells(Vector3Int initialPosition, Vector2Int size)
    {
        for (int xOffset = 0; xOffset < size.x; xOffset++)
        {
            for (int yOffset = 0; yOffset < size.y; yOffset++)
            {
                Vector3Int cell = new Vector3Int(initialPosition.x + xOffset, initialPosition.y + yOffset,
                    initialPosition.z);
                if (!occupiedPositions.Contains(cell))
                {
                    occupiedPositions.Add(cell); // Add the position to the list
                }
            }
        }
    }

    private GameObject GetMovableObjectAtPosition(Vector3Int gridPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(tilemap.GetCellCenterWorld(gridPosition), Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    public Vector3Int[] UpdateObjectOccupiedCells(Vector3Int oldPosition, Vector3Int newPosition, Vector2Int size)
    {
        // Remove the cells previously occupied by the object
        for (int xOffset = 0; xOffset < size.x; xOffset++)
        {
            for (int yOffset = 0; yOffset < size.y; yOffset++)
            {
                Vector3Int cell = new Vector3Int(oldPosition.x + xOffset, oldPosition.y + yOffset, oldPosition.z);
                occupiedPositions.Remove(cell);
            }
        }

        // Add the cells newly occupied by the object
        for (int xOffset = 0; xOffset < size.x; xOffset++)
        {
            for (int yOffset = 0; yOffset < size.y; yOffset++)
            {
                Vector3Int cell = new Vector3Int(newPosition.x + xOffset, newPosition.y + yOffset, newPosition.z);
                occupiedPositions.Add(cell);
            }
        }

        return occupiedPositions.ToArray();
    }
}