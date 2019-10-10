using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class MapBuilder : MonoBehaviour
{
    public static MapBuilder Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CreateMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Dimension")]
    public int m_Width = 6;
    public int m_Depth = 5;
    public float m_TileSize = 5.0f;

    [Header("Tiles")]
    public GameObject[] m_CornersTile;
    public GameObject[] m_LinesTile;
    public GameObject[] m_GroundsTile;

    [Header("Obstacles")]
    public GameObject[] m_Obstacles;
    public float m_ObstacleRate = 0.3f;
    public float m_Noise = 0.2f;

    [Header("Spawners")]
    public string m_SpawnerTag = "Ground";
    public List<Vector3> m_SpawnerPoints = new List<Vector3>();

    [Header("Manager")]
    public GameManager m_GameManager;
    private GameObject m_ObstaclesParent;

    private void Start()
    {
        m_GameManager.gameObject.SetActive(true);
    }

    private void CreateMap()
    {
        UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();

        float halfWidth = (m_Width * m_TileSize - m_TileSize) * 0.5f;
        float halfDepth = (m_Depth * m_TileSize - m_TileSize) * 0.5f;

        m_ObstaclesParent = new GameObject("Obstacles");
        m_ObstaclesParent.transform.parent = transform;

        for (int x = 0; x < m_Width; x++)
        {
            for (int z = 0; z < m_Depth; z++)
            {
                Vector3 position = Vector3.zero;
                position.x = -halfWidth + x * m_TileSize;
                position.z = -halfDepth + z * m_TileSize;

                if (!InstantiateBorderTile(x, z, position))
                {
                    InstantiateLineTile(x, z, position);
                    InstantiateGroundTile(x, z, position);
                }

                CreateObstacle(position);
            }

            m_SpawnerPoints.Shuffle();
        }

        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
    }

    private void InstantiateGroundTile(int x, int z, Vector3 position)
    {
        if (x == 0 || z == 0 || x == m_Width - 1 || z == m_Depth - 1)
            return;

        GameObject tile = GetRandomTile(m_GroundsTile, position);
        tile.transform.rotation = Quaternion.identity;
    }

    private bool InstantiateBorderTile(int x, int z, Vector3 position)
    {
        if (x == 0 && z == 0)
        {
            GameObject tile = GetRandomTile(m_CornersTile, position);
            tile.transform.rotation = Quaternion.Euler(Vector3.up * 270.0f);
            return true;
        }

        if (x == 0 && z == m_Depth - 1)
        {
            GameObject tile = GetRandomTile(m_CornersTile, position);
            tile.transform.rotation = Quaternion.Euler(Vector3.up * 0.0f);
            return true;
        }

        if (x == m_Width - 1 && z == 0)
        {
            GameObject tile = GetRandomTile(m_CornersTile, position);
            tile.transform.rotation = Quaternion.Euler(Vector3.up * 180.0f);
            return true;
        }

        if (x == m_Width - 1 && z == m_Depth - 1)
        {
            GameObject tile = GetRandomTile(m_CornersTile, position);
            tile.transform.rotation = Quaternion.Euler(Vector3.up * 90.0f);
            return true;
        }

        return false;
    }

    private void InstantiateLineTile(int x, int z, Vector3 position)
    {
        if (x == 0)
        {
            GameObject tile = GetRandomTile(m_LinesTile, position);
            tile.transform.rotation = Quaternion.Euler(Vector3.up * 0.0f);
        }

        if (z == 0)
        {
            GameObject tile = GetRandomTile(m_LinesTile, position);
            tile.transform.rotation = Quaternion.Euler(Vector3.up * 270.0f);
        }

        if (x == m_Width - 1)
        {
            GameObject tile = GetRandomTile(m_LinesTile, position);
            tile.transform.rotation = Quaternion.Euler(Vector3.up * 180.0f);
        }

        if (z == m_Depth - 1)
        {
            GameObject tile = GetRandomTile(m_LinesTile, position);
            tile.transform.rotation = Quaternion.Euler(Vector3.up * 90.0f);
        }
    }

    private GameObject GetRandomTile(GameObject[] tiles, Vector3 position)
    {
        int index = Random.Range(0, tiles.Length);

        GameObject tile = Instantiate(tiles[index]);
        tile.transform.position = position;
        tile.transform.parent = transform;

        return tile;
    }

    private void CreateObstacle(Vector3 position)
    {
        float rate = Random.Range(0.0f, 1.0f);

        if (rate < m_ObstacleRate)
        {
            int index = Random.Range(0, m_Obstacles.Length);

            position.y += 1.0f;
            position.x += Random.Range(-m_TileSize * m_Noise, m_TileSize * m_Noise);
            position.z += Random.Range(-m_TileSize * m_Noise, m_TileSize * m_Noise);

            float rotate = Random.Range(0.0f, 360.0f);
            GameObject obstacle = Instantiate(m_Obstacles[index], position, Quaternion.Euler(Vector3.up * rotate));
            obstacle.transform.parent = m_ObstaclesParent.transform;
        }
        else
        {
            m_SpawnerPoints.Add(position);
        }
    }
}