using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject[] blockPrefabs; // Array to hold the 5 different block prefabs
    public Transform[] spawnPoints;   // Array to hold the 8 spawn points
    public ObjectPool objectPool;     // Reference to the ObjectPool script
    public GameObject specialBlockPrefab; // Reference to the special block prefab (medicine)
    public Moves movescript;

    private int spawncounter = 0;
    private const int layers = 4;     // Number of layers to spawn
    private const int columns = 8;    // Number of columns (spawn points)
    private GameObject[,] grid = new GameObject[layers, columns]; // 2D array to represent the grid

    void Start()
    {
        InitializeObjectPool();
        SpawnBlocks();
    }

    void InitializeObjectPool()
    {
        objectPool.Initialize();
    }

    void SpawnBlocks()
    {
        for (int layer = 0; layer < layers; layer++)
        {
            for (int col = 0; col < columns; col++)
            {
                int randomIndex = Random.Range(0, blockPrefabs.Length);
                Vector3 spawnPosition = new Vector3(spawnPoints[col].position.x, spawnPoints[col].position.y - layer, spawnPoints[col].position.z);
                GameObject block = objectPool.GetFromPool(blockPrefabs[randomIndex].name, spawnPosition, Quaternion.identity);
                grid[layer, col] = block;
                block.AddComponent<Block>(); // Add the Block script to the spawned block
            }
        }
    }

    public void CheckMatches()
    {
        List<GameObject> blocksToDestroy = new List<GameObject>();

        // Check horizontal matches
        for (int row = 0; row < layers; row++)
        {
            for (int col = 0; col < columns - 2; col++)
            {
                if (grid[row, col] != null && grid[row, col + 1] != null && grid[row, col + 2] != null)
                {
                    if (grid[row, col].tag == grid[row, col + 1].tag && grid[row, col].tag == grid[row, col + 2].tag)
                    {
                        blocksToDestroy.Add(grid[row, col]);
                        blocksToDestroy.Add(grid[row, col + 1]);
                        blocksToDestroy.Add(grid[row, col + 2]);
                    }
                }
            }
        }

        // Check vertical matches
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < layers - 2; row++)
            {
                if (grid[row, col] != null && grid[row + 1, col] != null && grid[row + 2, col] != null)
                {
                    if (grid[row, col].tag == grid[row + 1, col].tag && grid[row, col].tag == grid[row + 2, col].tag)
                    {
                        blocksToDestroy.Add(grid[row, col]);
                        blocksToDestroy.Add(grid[row + 1, col]);
                        blocksToDestroy.Add(grid[row + 2, col]);
                    }
                }
            }
        }

        // Check diagonal matches (left to right)
        for (int row = 0; row < layers - 2; row++)
        {
            for (int col = 0; col < columns - 2; col++)
            {
                if (grid[row, col] != null && grid[row + 1, col + 1] != null && grid[row + 2, col + 2] != null)
                {
                    if (grid[row, col].tag == grid[row + 1, col + 1].tag && grid[row, col].tag == grid[row + 2, col + 2].tag)
                    {
                        blocksToDestroy.Add(grid[row, col]);
                        blocksToDestroy.Add(grid[row + 1, col + 1]);
                        blocksToDestroy.Add(grid[row + 2, col + 2]);
                    }
                }
            }
        }

        // Check diagonal matches (right to left)
        for (int row = 0; row < layers - 2; row++)
        {
            for (int col = 2; col < columns; col++)
            {
                if (grid[row, col] != null && grid[row + 1, col - 1] != null && grid[row + 2, col - 2] != null)
                {
                    if (grid[row, col].tag == grid[row + 1, col - 1].tag && grid[row, col].tag == grid[row + 2, col - 2].tag)
                    {
                        blocksToDestroy.Add(grid[row, col]);
                        blocksToDestroy.Add(grid[row + 1, col - 1]);
                        blocksToDestroy.Add(grid[row + 2, col - 2]);
                    }
                }
            }
        }

        // Destroy blocks and update grid
        foreach (GameObject block in blocksToDestroy)
        {
            int row = Mathf.RoundToInt(block.transform.position.y); // Assuming 1 unit per row
            int col = Mathf.RoundToInt(block.transform.position.x); // Assuming 1 unit per column

            if (grid[row, col] == block)
            {
                grid[row, col] = null;
            }

            objectPool.ReturnToPool(block); // Return the block to the pool
        }

        // No need to refill the grid, physics will handle the falling blocks
    }

    public void ShuffleBlocks()
    {
        StartCoroutine(ShuffleBlocksCoroutine());
        movescript.DecreaseMoveCount();
    }

    private IEnumerator ShuffleBlocksCoroutine()
    {
        // List of all possible color tags
        string[] colorTags = { "Yellow", "Red", "Blue", "Green", "Purple" };

        // Find all current blocks in the scene
        List<GameObject> currentBlocks = new List<GameObject>();

        foreach (string colorTag in colorTags)
        {
            currentBlocks.AddRange(GameObject.FindGameObjectsWithTag(colorTag));
        }

        // Shuffle the list of blocks
        currentBlocks.Shuffle();

        // Update the positions of shuffled blocks
        for (int i = 0; i < currentBlocks.Count; i++)
        {
            Vector3 position = currentBlocks[i].transform.position;
            objectPool.ReturnToPool(currentBlocks[i]); // Return the current block to the pool

            int randomIndex = Random.Range(0, blockPrefabs.Length);
            objectPool.GetFromPool(blockPrefabs[randomIndex].name, position, Quaternion.identity);
        }

        yield return null; // Spread the work over multiple frames
    }

    public void SpawnLineOfBlocks()
    {
        int specialBlockIndex = -1;

        if (spawncounter == 2)
        {
            specialBlockIndex = Random.Range(0, columns);
        }

        for (int col = 0; col < columns; col++)
        {
            Vector3 spawnPosition = spawnPoints[col].position;
            GameObject block;

            if (col == specialBlockIndex)
            {
                // Spawn a special block (medicine) instead of a random block
                block = Instantiate(specialBlockPrefab, spawnPosition, Quaternion.identity);
                Block blockComponent = block.AddComponent<Block>();
                blockComponent.IsSpecialBlock = true; // Mark the block as a special block
            }
            else
            {
                int randomIndex = Random.Range(0, blockPrefabs.Length);
                block = objectPool.GetFromPool(blockPrefabs[randomIndex].name, spawnPosition, Quaternion.identity);
                block.AddComponent<Block>(); // Add the Block script to the spawned block
            }
        }

        spawncounter++;
    }
}
