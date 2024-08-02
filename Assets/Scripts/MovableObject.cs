
using UnityEngine;

public abstract class MovableObject : MonoBehaviour
{   public abstract Vector2Int Size { get; }
    public bool isSelected;
    public Vector3Int position; //position on grid
    public GameObject highlighter;
    public bool isFirstClick = true;
    public Animator highlighterAnimator;

    private void Start()
    {
        if (highlighter != null && highlighterAnimator != null)
        {
            highlighter.SetActive(true);
            highlighterAnimator.enabled = true;
        }
        GameManager.OnObjectSelected += GameManager_OnObjectSelected;
        RandomPositioningGenerator.OnRandomPuzzleButtonCLicked += RandomPositioningGenerator_OnRandomPuzzleButtonCLicked;
    }
    private void GameManager_OnObjectSelected(object sender, System.EventArgs e)
    {
        if (isSelected == true)
        {
            ShowValidArrows();
            
        }
        if (isSelected == false)
        {
            HideArrows();
        }
    }
    private void HideArrows()
    {
        foreach (Transform childTransform in transform)
        {
            if (childTransform.CompareTag(Constants.arrowParentTag))
            {
                foreach (Transform arrow in childTransform)
                {
                    arrow.gameObject.SetActive(false);
                }
            }
        }
    }
    private void ShowValidArrows()
    {
        position = CalculateSelectedObjectPosition();
        bool canMoveUp = CanMoveUp();
        bool canMoveDown = CanMoveDown();
        bool canMoveLeft = CanMoveLeft();
        bool canMoveRight = CanMoveRight();

        Transform selectedArrowsParent = null;

        foreach (Transform childTransform in gameObject.transform)
        {
            if (childTransform.CompareTag(Constants.arrowParentTag))
            {
                selectedArrowsParent = childTransform;
                break;
            }
        }

        if (selectedArrowsParent != null)
        {
            foreach (Transform arrow in selectedArrowsParent)
            {
                if (arrow.CompareTag(Constants.upArrowTag))
                {
                    arrow.gameObject.SetActive(canMoveUp);
                }
                else if (arrow.CompareTag(Constants.downArrowTag))
                {
                    arrow.gameObject.SetActive(canMoveDown);
                }
                else if (arrow.CompareTag(Constants.leftArrowTag))
                {
                    arrow.gameObject.SetActive(canMoveLeft);
                }
                else if (arrow.CompareTag(Constants.rightArrowTag))
                {
                    arrow.gameObject.SetActive(canMoveRight);
                }
            }
        }
    }
    private Vector3Int CalculateSelectedObjectPosition()
    {
        Vector3 selectedObjectWorldPos = transform.position;
        Vector3Int selectedObjectGridPos = GameManager.Instance.tilemap.WorldToCell(selectedObjectWorldPos); //convert to gridcell
        return selectedObjectGridPos;
    }
    private bool CanMoveUp()
    {
        {
            int rowAbove = position.y + Size.y;

            if (rowAbove >= 6)
            {
                return false;
            }
            for (int x = position.x; x < position.x + Size.x; x++)
            {
                Vector3Int cellAbove = new Vector3Int(x, rowAbove, position.z);

                if (!GameManager.Instance.IsCellWithinBounds(cellAbove) || GameManager.Instance.IsCellOccupied(cellAbove))
                {
                    return false;
                }
            }
            return true;
        }
    }
    private bool CanMoveDown()
    {
        {
            int rowBelow = position.y - 1;

            if (rowBelow < 0)
            {
                return false; // Cannot move down as it's already at the bottom edge
            }
            for (int x = position.x; x < position.x + Size.x; x++)
            {
                Vector3Int cellBelow = new Vector3Int(x, rowBelow, position.z);
                if (!GameManager.Instance.IsCellWithinBounds(cellBelow) || GameManager.Instance.IsCellOccupied(cellBelow))
                {
                    return false;
                }
            }
            return true;
        }
    }
    private bool CanMoveLeft()
    {
        {
            int columnLeft = position.x - 1;
            if (columnLeft < 0)
            {
                return false;
            }
            for (int y = position.y; y < position.y + Size.y; y++)
            {
                Vector3Int cellToLeft = new Vector3Int(columnLeft, y, position.z);
                if (!GameManager.Instance.IsCellWithinBounds(cellToLeft) || GameManager.Instance.IsCellOccupied(cellToLeft))
                {
                    return false;
                }
            }
            return true;
        }
    }
    private bool CanMoveRight()
    {
        {
            int columnRight = position.x + Size.x;
            if (columnRight >= 6)
            {
                return false;
            }
            for (int y = position.y; y < position.y + Size.y; y++)
            {
                Vector3Int cellToRight = new Vector3Int(columnRight, y, position.z);
                if (!GameManager.Instance.IsCellWithinBounds(cellToRight) || GameManager.Instance.IsCellOccupied(cellToRight))
                {
                    return false;
                }
            }
            return true;
        }
    }
    public void MoveUp()
    {
        Vector3Int newPosition = new Vector3Int(position.x, position.y + 1, position.z);
        MoveTo(newPosition);
    }
    public void MoveDown()
    {
        Vector3Int newPosition = new Vector3Int(position.x, position.y - 1, position.z);
        MoveTo(newPosition);
    }
    public void MoveLeft()
    {
        Vector3Int newPosition = new Vector3Int(position.x - 1, position.y, position.z);            // Update the occupied cells for the selected object
        MoveTo(newPosition);
    }
    public void MoveRight()
    {
        Vector3Int newPosition = new Vector3Int(position.x + 1, position.y, position.z);
        MoveTo(newPosition);
    }
    private void MoveTo(Vector3Int newPosition)
    {
        MusicManager.instance.PlayMoveSound();
        transform.position = GameManager.Instance.tilemap.GetCellCenterWorld(newPosition);
        if ((this is Player) && (newPosition == GameManager.Instance.winningPosition))
        {
            GameManager.isGameOver = true;
            GameOverUI.Instance.ShowGameOver(false);
        }
        GameManager.Instance.UpdateObjectOccupiedCells(position, newPosition, Size);
        ShowValidArrows();
    }
    private void OnDestroy()
    {
        GameManager.OnObjectSelected -= GameManager_OnObjectSelected;
        RandomPositioningGenerator.OnRandomPuzzleButtonCLicked -= RandomPositioningGenerator_OnRandomPuzzleButtonCLicked;
    }

    private void RandomPositioningGenerator_OnRandomPuzzleButtonCLicked(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
        RemoveObjectOccupiedCells();
    }

    public void RemoveObjectOccupiedCells()
    {
        for (int xOffset = 0; xOffset < Size.x; xOffset++)
        {
            for (int yOffset = 0; yOffset < Size.y; yOffset++)
            {
                Vector3Int cell = new Vector3Int(position.x + xOffset, position.y + yOffset, position.z);
                GameManager.Instance.occupiedPositions.Remove(cell);
            }
        }
    }
    public void SetGameObject(GameObject gameObject)
    {
        gameObject.transform.position = GameManager.Instance.tilemap.GetCellCenterWorld(position);
        gameObject.transform.SetParent(GameManager.Instance.tilemap.transform);
    }
}