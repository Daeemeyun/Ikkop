using System.Collections.Generic;
using UnityEngine;

public class BSPDungeonGenerator : MonoBehaviour
{
    public float tileSize = 1f;
    public int width = 20;
    public int height = 20;
    public int maxDepth = 2;
    public int roomMinSize = 5;
    public GameObject roomPrefab;
    public GameObject playerPrefab;
    public GameObject exitPortalPrefab;
    public GameObject enemyPrefab;
    public GameObject corridorPrefab;
    public GameObject wallPrefab;
    public GameObject potPrefab;
    public GameObject invisibleWallPrefab;
    public GameObject bossPrefab;

    private List<Room> rooms = new List<Room>();
    private HashSet<Vector2Int> corridorPoints = new HashSet<Vector2Int>();

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        BSPNode root = new BSPNode(0, 0, width, height, 0, maxDepth, roomMinSize);
        root.Split();
        rooms = root.GetRooms();

        for (int i = 0; i < rooms.Count; i++)
        {
            Room room = rooms[i];

            // applying the tileSize scaling to world position
            // Vector3 worldPos = new Vector3(room.x * tileSize, room.y * tileSize, 0);
            float roomCenterX = (room.x + room.width / 2f) * tileSize;
            float roomCenterY = (room.y + room.height / 2f) * tileSize;
            Vector3 worldPos = new Vector3(roomCenterX, roomCenterY, 0);
            Vector3 centerWorldPos = new Vector3((room.x + room.width / 2f) * tileSize, (room.y + room.height / 2f) * tileSize, 0);

            // instantiating room code
            for (int x = 0; x < room.width; x++)
            {
                for (int y = 0; y < room.height; y++)
                {
                    Vector3 tilePos = new Vector3((room.x + x) * tileSize, (room.y + y) * tileSize, 0.1f);
                    Instantiate(roomPrefab, tilePos, Quaternion.identity);
                }
            }

            // adding wall boundaries (prevents player from walking into the abyss)
            float xMin = room.x;
            float xMax = room.x + room.width;
            float yMin = room.y;
            float yMax = room.y + room.height;
            
            if (i == 0)
            {
                float pokkiCenterX = (room.x + room.width / 2f) * tileSize;
                float pokkiCenterY = (room.y + room.height / 2f) * tileSize;
                GameObject pokki = Instantiate(playerPrefab, new Vector3(pokkiCenterX, pokkiCenterY, -0.1f), Quaternion.identity);
                pokki.tag = "Player";

                // attaching the faint screen to the player
                PlayerHealth health = pokki.GetComponent<PlayerHealth>();
                health.faintScreen = GameObject.Find("FaintScreen");
            }
            else
            {
                // locating the wall tile where a corridor touches the room
                Vector2Int? entryPoint = null;

                for (int x = room.x - 1; x <= room.x + room.width; x++)
                {
                    for (int y = room.y - 1; y <= room.y + room.height; y++)
                    {
                        bool isEdgeX = x == room.x - 1 || x == room.x + room.width;
                        bool isEdgeY = y == room.y - 1 || y == room.y + room.height;

                        if ((isEdgeX || isEdgeY) && corridorPoints.Contains(new Vector2Int(x, y)))
                        {
                            entryPoint = new Vector2Int(x, y);
                            break;
                        }
                    }
                    if (entryPoint.HasValue) break;
                }

                // logic check to see if this is the last room a.k.a the boss room
                if (i == rooms.Count - 1)
                {
                    // spawning boss in the center of the boss room
                    Vector3 roomCenter = new Vector3(
                        (room.x + room.width / 2f) * tileSize,
                        (room.y + room.height / 2f) * tileSize,
                        0
                    );

                    if (bossPrefab != null)
                    {
                        GameObject boss = Instantiate(bossPrefab, roomCenter, Quaternion.identity);
                        Debug.Log("Boss spawned at: " + roomCenter);
                    }
                    else
                    {
                        Debug.LogWarning("Boss prefab not assigned!");
                    }
                }
                else
                {
                    // normal enemy spawning logic for non-boss rooms
                    int enemyCount = Random.Range(2, 4);

                    for (int e = 0; e < enemyCount; e++)
                    {
                        float randX = Random.Range(room.x + 1, room.x + room.width - 1) * tileSize;
                        float randY = Random.Range(room.y + 1, room.y + room.height - 1) * tileSize;

                        Instantiate(enemyPrefab, new Vector3(randX, randY, 0), Quaternion.identity);
                    }
                }

                // destroyable pot spawn code (as long as not the first room which is the spawn room)
                int potCount = Random.Range(1, 3);

                for (int p = 0; p < potCount; p++)
                {
                    float randX = Random.Range(room.x + 1, room.x + room.width - 1) * tileSize;
                    float randY = Random.Range(room.y + 1, room.y + room.height - 1) * tileSize;

                    Instantiate(potPrefab, new Vector3(randX, randY, 0), Quaternion.identity);
                }
            }
        }

        // connecting rooms using BSP logic
        root.ConnectRooms(this);

        // function to help navigate any inviisble walls at T junctions
        SpawnWallsBesideCorridors();

        Vector3 exitPos = new Vector3(rooms[rooms.Count - 1].CenterX * tileSize, rooms[rooms.Count - 1].CenterY * tileSize - tileSize, 0);
        // Instantiate(exitPortalPrefab, exitPos, Quaternion.identity); < used to use this code to place a permanent portal on the last room, can use for debugging

        // placing walls around all room boundaries with the exception of where the corridors are
        foreach (Room room in rooms)
        {
            for (int x = room.x - 1; x <= room.x + room.width; x++)
            {
                for (int y = room.y - 1; y <= room.y + room.height; y++)
                {
                    bool isEdgeX = x == room.x - 1 || x == room.x + room.width;
                    bool isEdgeY = y == room.y - 1 || y == room.y + room.height;

                    if ((isEdgeX || isEdgeY) && !corridorPoints.Contains(new Vector2Int(x, y)))
                    {
                        Instantiate(wallPrefab, new Vector3(x * tileSize, y * tileSize, 0), Quaternion.identity);
                    }
                }
            }
        }
    }

    public void CreateCorridor(Vector2 from, Vector2 to)
    {
        Vector2 current = from;

        bool horizontalFirst = Random.value > 0.5f;

        if (horizontalFirst)
        {
            // for the horizontal
            while (Mathf.RoundToInt(current.x) != Mathf.RoundToInt(to.x))
            {
                current.x += Mathf.Sign(to.x - current.x);
                Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(current.x), Mathf.RoundToInt(current.y));
                corridorPoints.Add(gridPos);

                // rotating it 90 degrees
                Quaternion rotation = Quaternion.Euler(0, 0, 90);
                Instantiate(corridorPrefab, new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0.1f), rotation);
            }

            // for the vertical
            while (Mathf.RoundToInt(current.y) != Mathf.RoundToInt(to.y))
            {
                current.y += Mathf.Sign(to.y - current.y);
                Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(current.x), Mathf.RoundToInt(current.y));
                corridorPoints.Add(gridPos);

                // no need to rotate as default position is vertical already
                Quaternion rotation = Quaternion.identity;
                Instantiate(corridorPrefab, new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0.1f), rotation);
            }
        }
        else
        {
            // for vertical
            while (Mathf.RoundToInt(current.y) != Mathf.RoundToInt(to.y))
            {
                current.y += Mathf.Sign(to.y - current.y);
                Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(current.x), Mathf.RoundToInt(current.y));
                corridorPoints.Add(gridPos);

                Quaternion rotation = Quaternion.identity;
                Instantiate(corridorPrefab, new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0.1f), rotation);
            }

            // for horizontal
            while (Mathf.RoundToInt(current.x) != Mathf.RoundToInt(to.x))
            {
                current.x += Mathf.Sign(to.x - current.x);
                Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(current.x), Mathf.RoundToInt(current.y));
                corridorPoints.Add(gridPos);

                Quaternion rotation = Quaternion.Euler(0, 0, 90);
                Instantiate(corridorPrefab, new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0.1f), rotation);
            }
        }
    }

    // defining the function for spawning invisible walls
    private void SpawnInvisibleWall(Vector2Int gridPos)
    {
        // if already a corridor or has a corridor tile, dont spawn the invisible wall
        if (corridorPoints.Contains(gridPos)) return;

        // checking if there is already corridor
        Vector3 worldPos = new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0.1f);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null && hit.CompareTag("Corridor")) return;

        Instantiate(invisibleWallPrefab, new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0), Quaternion.identity);
    }


    private void SpawnWallsBesideCorridors()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),   // right
            new Vector2Int(-1, 0),  // left
            new Vector2Int(0, 1),   // up
            new Vector2Int(0, -1),  // down
        };

        foreach (Vector2Int point in corridorPoints)
        {
            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = point + dir;

                if (corridorPoints.Contains(neighbor) || IsPointInsideAnyRoom(neighbor))
                    continue;

                // instantiate invisible wall
                Vector3 wallPos = new Vector3(neighbor.x * tileSize, neighbor.y * tileSize, 0);
                GameObject wall = Instantiate(invisibleWallPrefab, wallPos, Quaternion.identity);

                // debugging assitance function w color
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(0f, 0f, 0f, 0.0f); // for debugging invisible wall (shows visual (R,G,B,A))
                }
            }
        }
    }

    private bool IsPointInsideAnyRoom(Vector2Int point)
    {
        foreach (var room in rooms)
        {
            if (point.x >= room.x && point.x < room.x + room.width &&
                point.y >= room.y && point.y < room.y + room.height)
            {
                return true;
            }
        }
        return false;
    }

// ROOM + NODE CLASS FROM HERE ONWARDS

    class Room
    {
        public int x, y, width, height;
        public int CenterX => x + width / 2;
        public int CenterY => y + height / 2;
    }

    class BSPNode
    {
        public int x, y, width, height, depth;
        public BSPNode left, right;
        public Room room;
        int maxDepth;
        int minSize;

        public BSPNode(int x, int y, int width, int height, int depth, int maxDepth, int minSize)
        {
            this.x = x; this.y = y; this.width = width; this.height = height;
            this.depth = depth; this.maxDepth = maxDepth; this.minSize = minSize;
            
        }

        public void Split()
        {
            if (depth >= maxDepth || width < minSize * 2 || height < minSize * 2) return;

            bool splitHorizontally = width < height;

            if (splitHorizontally)
            {
                int split = Random.Range(minSize, height - minSize);
                left = new BSPNode(x, y, width, split, depth + 1, maxDepth, minSize);
                right = new BSPNode(x, y + split, width, height - split, depth + 1, maxDepth, minSize);
            }
            else
            {
                int split = Random.Range(minSize, width - minSize);
                left = new BSPNode(x, y, split, height, depth + 1, maxDepth, minSize);
                right = new BSPNode(x + split, y, width - split, height, depth + 1, maxDepth, minSize);
            }

            left.Split();
            right.Split();
        }

        public List<Room> GetRooms()
        {
            List<Room> result = new List<Room>();
            if (left == null && right == null)
            {
                int clampedWidth = Mathf.Clamp(width - 2, 6, 12);
                int clampedHeight = Mathf.Clamp(height - 2, 6, 12);
                room = new Room
                {
                    x = x + 1,
                    y = y + 1,
                    width = clampedWidth,
                    height = clampedHeight
                };

                result.Add(room);
            }
            else
            {
                if (left != null) result.AddRange(left.GetRooms());
                if (right != null) result.AddRange(right.GetRooms());
            }
            return result;
        }

        public Room GetRandomRoom()
        {
            if (room != null)
                return room;

            List<Room> leftRooms = left?.GetRooms();
            List<Room> rightRooms = right?.GetRooms();

            List<Room> allRooms = new List<Room>();
            if (leftRooms != null) allRooms.AddRange(leftRooms);
            if (rightRooms != null) allRooms.AddRange(rightRooms);

            if (allRooms.Count > 0)
                return allRooms[Random.Range(0, allRooms.Count)];

            return null;
        }

        public void ConnectRooms(BSPDungeonGenerator generator)
        {
            if (left != null && right != null)
            {
                left.ConnectRooms(generator);
                right.ConnectRooms(generator);

                Room leftRoom = left.GetRandomRoom();
                Room rightRoom = right.GetRandomRoom();

                if (leftRoom != null && rightRoom != null)
                {
                    Vector2 leftCenter = new Vector2(leftRoom.CenterX, leftRoom.CenterY);
                    Vector2 rightCenter = new Vector2(rightRoom.CenterX, rightRoom.CenterY);
                    generator.CreateCorridor(leftCenter, rightCenter);
                }
            }
        }
    }
}
