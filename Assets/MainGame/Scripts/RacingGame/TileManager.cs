using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Transform RoadHolder;
    [Tooltip("Prefab Tiles for road")]
    public GameObject[] tilePrefabs;        // Array of tiles
    /*
        [0] -> Grass            -> Green
        [1] -> Middle road      -> Blue
        [2] -> Road closing     -> Red
     */

    public float zSpawn = 0;        // Place where the tile will be respawned
    [Tooltip("Size of the Tiles")]
    public float tileLength;
    [Tooltip("Number of roads to be spawned when starting the game.")]
    public int numberOfRoads;

    [Tooltip("Actual Player (With Player Controller)")]
    public Transform playerTransform;
    //List of tiles on scene
    private List<GameObject> activeTiles = new List<GameObject>();

    private Vector3 mapInstantiation;

    // Log1 changes **************************************
    public GameObject[] obstaclePrefabs;
    /*
        [0] -> Destructible obstacle
        [1] -> Hard obstacle
     */
    private List<GameObject> activeObstacles = new List<GameObject>();
    public Transform ObstacleHolder;
    public bool obstacleGenerated = false;

    // #Log 0611
    public int posCorrectObstacle;
    // End Log1 changes **********************************

    void Start()
    {
        mapInstantiation = playerTransform.position;
        // To spawn the first 10 tiles at the beginning
        for (int i = 0; i < 10; i++)
        {
            SpawnTile();
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Si el carrito se acerca a la orilla, creamos mmás tiles y borramos los que ya pasamos.
        if (playerTransform.position.z > zSpawn - (5 * tileLength))
        {
            SpawnTile();
            DeleteTile(activeTiles, numberOfRoads + 2);
        }
        // Log1
        if (Input.GetKeyDown("space"))
        {
            if (obstacleGenerated)
            {
                DeleteTile(activeObstacles, numberOfRoads);
                obstacleGenerated = false;
            }
            print("Entro a generacion de obstaculos");
            GenerateObstacle();
            obstacleGenerated = true;
        }
    }

    // Spawn a row of tiles in front of the player.
    public void SpawnTile()
    {
        //Instantiate first tile on the row
        //Depending on zSpawn position, we positionate the z height of the row.
        //Depending on cart's first appearance position, we spawn the tiles on X.
        Vector3 posTile = new Vector3();
        posTile.z = transform.forward.z * zSpawn;
        posTile.x = mapInstantiation.x - tileLength;
        AddRoad(tilePrefabs[0], activeTiles, posTile, Quaternion.identity, RoadHolder, "Road");

        for (int j = 0; j < numberOfRoads - 1; j++)
        {
            posTile.x += tileLength;
            posTile.z = transform.forward.z * zSpawn;
            AddRoad(tilePrefabs[1], activeTiles, posTile, Quaternion.identity, RoadHolder, "Road");
        }
        posTile.x += tileLength;
        AddRoad(tilePrefabs[2], activeTiles, posTile, Quaternion.identity, RoadHolder, "Road");
        posTile.x += tileLength;
        AddRoad(tilePrefabs[0], activeTiles, posTile, Quaternion.identity, RoadHolder, "Road");

        zSpawn += tileLength;
    }

    // To delete the tiles already passed by the player
    private void DeleteTile(List<GameObject> tileHolder, int numObjects)  // Log1 changes
    {
        // We delete the number of tiles in a row, which consists of the number of roads + outside tiles (2).
        for (int j = 0; j < numObjects; j++)
        {
            Destroy(tileHolder[0]);
            tileHolder.RemoveAt(0);
        }
    }


    //Function to Add a tile to a specific list, with rotation position and parent.
    private void AddRoad(GameObject tile, List<GameObject> tileHolder, Vector3 position, Quaternion rotation, Transform parent, string objType)
    {
        GameObject obj = Instantiate(tile, position, rotation, parent);
        obj.tag = objType;
        tileHolder.Add(obj);
    }

    public void GenerateObstacle()
    {
        if (obstacleGenerated)
        {
            DeleteTile(activeObstacles, numberOfRoads);
            obstacleGenerated = false;
        }
        Vector3 posObstacle = new Vector3();
        posCorrectObstacle = Random.Range(0, numberOfRoads);

        posObstacle.z = transform.forward.z * (zSpawn - tileLength);
        posObstacle.x = mapInstantiation.x - tileLength;

        for (int j = 0; j < numberOfRoads; j++)
        {
            posObstacle.x += tileLength;
            posObstacle.z = transform.forward.z * zSpawn;

            if (j == posCorrectObstacle)
            {
                print("Numero aleatorio -> " + posCorrectObstacle);
                AddRoad(obstaclePrefabs[0], activeObstacles, posObstacle, Quaternion.identity, ObstacleHolder, "Untagged");
            }
            else AddRoad(obstaclePrefabs[1], activeObstacles, posObstacle, Quaternion.identity, ObstacleHolder, "Obstacle");
        }
        obstacleGenerated = true;
    }

    public Vector3 GetMapInstantiation()
    {
        return mapInstantiation;
    }


}