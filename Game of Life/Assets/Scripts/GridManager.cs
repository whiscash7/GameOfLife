using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 100;
    public int height = 100;

    private float squareSize = 1f;

    public Unit unitPrefab;

    public Unit[,] grid;

    public bool isPaused = true;
    public float tickSpeed = 10f;
    private float nextTickTime = 0f;

    private void Awake() {
        grid = new Unit[width, height];

        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                Unit unit = Instantiate(unitPrefab, new Vector3(i * squareSize, j * squareSize, 0f), Quaternion.identity);
                unit.Initialize(this, new Vector2Int(i, j));
            }
        }
    }
    public Unit GetUnitAt(Vector2Int pos) {
        return grid[pos.x, pos.y];
    }
    public void SetUnitAt(Vector2Int pos, Unit unit) {
        grid[pos.x, pos.y] = unit;
    }

    public Vector2Int WrapAround(int x, int y) {
        int wrappedX = (x % width + width) % width;
        int wrappedY = (y % height + height) % height;
        return new Vector2Int(wrappedX, wrappedY);
    }

    private void Update() {
        if (!isPaused) {
            if (Time.time >= nextTickTime) {
                Steps();
                nextTickTime = Time.time + 1 / tickSpeed;
            }
            return;
        }

        if (Input.GetKeyDown("space")) {
            Steps();
        }
    }

    public void Steps() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y]?.StepOne();
            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y]?.StepTwo();
            }
        }
    }

    public void pauseToggle(bool toggleValue) { isPaused = toggleValue; }
    public void tickSpeedSlider(float sliderValue) { tickSpeed = sliderValue; }
}
