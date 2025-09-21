using UnityEngine;
using System.Collections.Generic;

public class AStar2D : MonoBehaviour
{
    public GridBaker2D grid;

    class PQ
    {
        List<(int id, float f)> heap = new();
        public int Count => heap.Count;
        public void Push(int id, float f)
        {
            heap.Add((id, f));
            int i = heap.Count - 1;
            while (i > 0)
            {
                int p = (i - 1) >> 1;
                if (heap[p].f <= f) break;
                (heap[i], heap[p]) = (heap[p], heap[i]); i = p;
            }
        }
        public int Pop()
        {
            var ret = heap[0].id;
            var last = heap[^1];
            heap.RemoveAt(heap.Count - 1);
            if (heap.Count == 0) return ret;
            heap[0] = last;
            int i = 0;
            while (true)
            {
                int l = i * 2 + 1, r = l + 1, s = i;
                if (l < heap.Count && heap[l].f < heap[s].f) s = l;
                if (r < heap.Count && heap[r].f < heap[s].f) s = r;
                if (s == i) break;
                (heap[i], heap[s]) = (heap[s], heap[i]); i = s;
            }
            return ret;
        }
    }

    public bool FindPath(Vector2 start, Vector2 goal, List<Vector2> outPath)
    {
        outPath.Clear();
        if (grid == null || grid.walkable == null) return false;

        if (!grid.WorldToGrid(start, out int sx, out int sy)) return false;
        if (!grid.WorldToGrid(goal, out int gx, out int gy)) return false;
        if (!grid.IsWalkable(sx, sy) || !grid.IsWalkable(gx, gy)) return false;

        int nx = grid.nx, ny = grid.ny;
        int N = nx * ny;

        var came = new int[N]; for (int i = 0; i < N; i++) came[i] = -1;
        var gCost = new float[N]; for (int i = 0; i < N; i++) gCost[i] = float.PositiveInfinity;
        var fCost = new float[N]; for (int i = 0; i < N; i++) fCost[i] = float.PositiveInfinity;
        var open = new PQ();
        var closed = new bool[N];

        int startId = grid.Idx(sx, sy);
        int goalId = grid.Idx(gx, gy);

        gCost[startId] = 0f;
        fCost[startId] = Heuristic(sx, sy, gx, gy);
        open.Push(startId, fCost[startId]);

        // neighbor buffer (max 8)
        Vector2Int[] neigh = new Vector2Int[8];

        while (open.Count > 0)
        {
            int cur = open.Pop();
            if (cur == goalId)
                return Reconstruct(cur, came, outPath);

            if (closed[cur]) continue;
            closed[cur] = true;

            int cx = cur % nx;
            int cy = cur / nx;

            int count = grid.GetNeighbors(cx, cy, neigh);
            for (int i = 0; i < count; i++)
            {
                var n = neigh[i];
                int nid = grid.Idx(n.x, n.y);
                if (closed[nid]) continue;

                float w = (n.x != cx && n.y != cy) ? 1.41421356f : 1f; // diag vs straight
                float tentative = gCost[cur] + w;

                if (tentative < gCost[nid])
                {
                    came[nid] = cur;
                    gCost[nid] = tentative;
                    fCost[nid] = tentative + Heuristic(n.x, n.y, gx, gy);
                    open.Push(nid, fCost[nid]);
                }
            }
        }
        return false;
    }

    bool Reconstruct(int cur, int[] came, List<Vector2> outPath)
    {
        outPath.Clear();
        var stack = new Stack<int>();
        while (cur != -1) { stack.Push(cur); cur = came[cur]; }
        while (stack.Count > 0)
        {
            int id = stack.Pop();
            int nx = grid.nx;
            int x = id % nx; int y = id / nx;
            outPath.Add(grid.GridToWorld(x, y));
        }
        return outPath.Count > 0;
    }

    float Heuristic(int x, int y, int gx, int gy)
    {
        // Octile distance (good for 8-way)
        int dx = Mathf.Abs(x - gx), dy = Mathf.Abs(y - gy);
        return (dx + dy) + (1.41421356f - 2f) * Mathf.Min(dx, dy);
    }
}
