using UnityEngine;
using System;

[DefaultExecutionOrder(-50)]
public class GridBaker2D : MonoBehaviour
{
    [Header("Bake Area (world space)")]
    public Vector2 center = Vector2.zero;
    public Vector2 size = new Vector2(40, 25);

    [Header("Resolution")]
    [Min(0.05f)] public float cellSize = 0.25f;

    [Header("Collision")]
    public LayerMask obstacleMask;       // set to Obstacles + anything solid
    [Range(0f, 0.49f)] public float inflate = 0.1f; // extra padding around obstacles

    [Header("Neighbors")]
    public bool allowDiagonal = true;

    // baked data
    public bool[] walkable; // length = nx * ny
    public int nx, ny;
    Vector2 origin;

    public int Count => nx * ny;

    void OnValidate()
    {
        nx = Mathf.Max(1, Mathf.RoundToInt(size.x / cellSize));
        ny = Mathf.Max(1, Mathf.RoundToInt(size.y / cellSize));
        origin = center - size * 0.5f;
    }

    void Awake() => Bake();

    [ContextMenu("Bake Now")]
    public void Bake()
    {
        OnValidate();
        walkable = new bool[Count];
        var half = Vector2.one * (cellSize * 0.5f - inflate);
        var box = new Vector2(Mathf.Max(0.01f, half.x * 2), Mathf.Max(0.01f, half.y * 2));

        int i = 0;
        for (int y = 0; y < ny; y++)
        {
            for (int x = 0; x < nx; x++, i++)
            {
                Vector2 p = GridToWorld(x, y);
                var hit = Physics2D.OverlapBox(p, box, 0f, obstacleMask);
                walkable[i] = (hit == null);
            }
        }
    }

    public bool WorldToGrid(Vector2 p, out int x, out int y)
    {
        Vector2 local = p - origin;
        x = Mathf.FloorToInt(local.x / cellSize);
        y = Mathf.FloorToInt(local.y / cellSize);
        return x >= 0 && y >= 0 && x < nx && y < ny;
    }

    public Vector2 GridToWorld(int x, int y)
    {
        return origin + new Vector2((x + 0.5f) * cellSize, (y + 0.5f) * cellSize);
    }

    public int Idx(int x, int y) => y * nx + x;

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < nx && y < ny;

    public bool IsWalkable(int x, int y)
    {
        if (!InBounds(x, y)) return false;
        return walkable[Idx(x, y)];
    }

    public int GetNeighbors(int x, int y, Vector2Int[] buffer)
    {
        // buffer must have length >= 8
        int count = 0;

        // 4-way
        TryAdd(x + 1, y);
        TryAdd(x - 1, y);
        TryAdd(x, y + 1);
        TryAdd(x, y - 1);

        // diagonals
        if (allowDiagonal)
        {
            TryAdd(x + 1, y + 1);
            TryAdd(x - 1, y + 1);
            TryAdd(x + 1, y - 1);
            TryAdd(x - 1, y - 1);
        }

        return count;

        void TryAdd(int gx, int gy)
        {
            if (InBounds(gx, gy) && IsWalkable(gx, gy))
            {
                buffer[count++] = new Vector2Int(gx, gy);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        OnValidate();
        // bounds
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = new Color(0, 1, 0, 0.7f);
        Gizmos.DrawWireCube(center, size);

        if (walkable == null || walkable.Length != Count) return;

        // draw coarse grid (skip most cells for performance in editor)
        int step = Mathf.Max(1, Mathf.RoundToInt(0.5f / Mathf.Max(0.05f, cellSize)));
        for (int y = 0; y < ny; y += step)
        for (int x = 0; x < nx; x += step)
        {
            var p = GridToWorld(x, y);
            Gizmos.color = IsWalkable(x, y) ? new Color(0, 0.6f, 1f, 0.15f) : new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawCube(p, Vector3.one * cellSize * 0.95f);
        }
    }
}
