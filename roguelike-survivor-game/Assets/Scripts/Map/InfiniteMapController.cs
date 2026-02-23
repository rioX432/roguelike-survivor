using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// 3x3 chunk ring-buffer infinite map.
    /// Repositions Transform.position only — no Tilemap.SetTile() calls.
    /// </summary>
    public class InfiniteMapController : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Sprite _backgroundTile;
        [SerializeField] private float _chunkSize = 12f; // world-unit size per chunk (should match sprite size)
        [SerializeField] private int _sortingOrder = -10;

        // 3x3 grid of chunks
        private Transform[,] _chunks;
        private Vector2 _lastPlayerChunk;

        public void SetPlayer(Transform player) => _player = player;
        public void SetTile(Sprite tile) => _backgroundTile = tile;

        private void Awake()
        {
            // BuildChunkGrid is deferred to Start() so SetTile/SetPlayer can be called first
        }

        private void Start()
        {
            BuildChunkGrid();

            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (_player == null) return;

            _lastPlayerChunk = WorldToChunkCoord(_player.position);
            SnapChunksToPlayer();
        }

        private void LateUpdate()
        {
            if (_player == null) return;

            Vector2 currentChunk = WorldToChunkCoord(_player.position);
            if (currentChunk != _lastPlayerChunk)
            {
                ShiftChunks(currentChunk - _lastPlayerChunk);
                _lastPlayerChunk = currentChunk;
            }
        }

        private void BuildChunkGrid()
        {
            _chunks = new Transform[3, 3];
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    var go = new GameObject($"Chunk_{x}_{y}");
                    go.transform.SetParent(transform);

                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = _backgroundTile;
                    sr.drawMode = SpriteDrawMode.Tiled;
                    sr.size = new Vector2(_chunkSize, _chunkSize);
                    sr.sortingOrder = _sortingOrder;

                    _chunks[x, y] = go.transform;
                }
            }
        }

        private void SnapChunksToPlayer()
        {
            Vector2 origin = _lastPlayerChunk * _chunkSize;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    float px = origin.x + (x - 1) * _chunkSize;
                    float py = origin.y + (y - 1) * _chunkSize;
                    _chunks[x, y].position = new Vector3(px, py, 0f);
                }
            }
        }

        /// <summary>
        /// Called when the player crosses a chunk boundary.
        /// Moves the far row/column to the opposite side.
        /// </summary>
        private void ShiftChunks(Vector2 delta)
        {
            // Shift in X
            if (delta.x > 0)      MoveColumn(0, 2);   // left col → right
            else if (delta.x < 0) MoveColumn(2, 0);   // right col → left

            // Shift in Y
            if (delta.y > 0)      MoveRow(0, 2);      // bottom row → top
            else if (delta.y < 0) MoveRow(2, 0);      // top row → bottom
        }

        /// <summary>Teleport source column to the opposite side of the dest column.</summary>
        private void MoveColumn(int srcX, int destX)
        {
            float destXPos = _chunks[destX, 1].position.x;
            float sign = destX > srcX ? 1f : -1f;

            for (int y = 0; y < 3; y++)
            {
                Vector3 p = _chunks[srcX, y].position;
                p.x = destXPos + sign * _chunkSize;
                _chunks[srcX, y].position = p;
            }

            // Swap column references so the grid remains consistent
            for (int y = 0; y < 3; y++)
            {
                var tmp = _chunks[srcX, y];
                _chunks[srcX, y] = _chunks[destX, y];
                _chunks[destX, y] = tmp;
            }
        }

        /// <summary>Teleport source row to the opposite side of the dest row.</summary>
        private void MoveRow(int srcY, int destY)
        {
            float destYPos = _chunks[1, destY].position.y;
            float sign = destY > srcY ? 1f : -1f;

            for (int x = 0; x < 3; x++)
            {
                Vector3 p = _chunks[x, srcY].position;
                p.y = destYPos + sign * _chunkSize;
                _chunks[x, srcY].position = p;
            }

            for (int x = 0; x < 3; x++)
            {
                var tmp = _chunks[x, srcY];
                _chunks[x, srcY] = _chunks[x, destY];
                _chunks[x, destY] = tmp;
            }
        }

        private Vector2 WorldToChunkCoord(Vector3 worldPos)
        {
            return new Vector2(
                Mathf.Round(worldPos.x / _chunkSize),
                Mathf.Round(worldPos.y / _chunkSize)
            );
        }
    }
}
