using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BlockLinker : MonoBehaviour
{
    public static BlockLinker instance;
    public Moves movescript;
    public BlockSpawner spawner;

    public bool isLinking = false;
    private List<Block> linkedBlocks = new List<Block>();
    private List<GameObject> selectedGameObjects = new List<GameObject>(); // List to store selected blocks based on GameObject references

    private const float MAX_DISTANCE = 1.5f; // Maximum allowed distance between blocks

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartLinking(Block startBlock)
    {
        isLinking = true;
        AddToLink(startBlock);
    }

    public void AddToLink(Block block)
    {
        if (!block.isLinked && !selectedGameObjects.Contains(block.gameObject))
        {
            if (linkedBlocks.Count > 0)
            {
                // Get the last linked block
                Block lastLinkedBlock = linkedBlocks[linkedBlocks.Count - 1];

                // Check the distance between the last linked block and the current block
                float distance = Vector3.Distance(lastLinkedBlock.transform.position, block.transform.position);
                if (distance > MAX_DISTANCE)
                {
                    return; // Block is too far, do not link
                }
            }

            linkedBlocks.Add(block);
            block.isLinked = true;
            selectedGameObjects.Add(block.gameObject);

            HighlightBlock(block);

            // Check adjacent blocks
            CheckAdjacentBlocks(block);
        }
    }

    private void CheckAdjacentBlocks(Block block)
    {
        Vector3[] directions = {
            Vector3.up, Vector3.down, Vector3.left, Vector3.right, // Horizontal and vertical
            new Vector3(1, 1, 0), new Vector3(-1, -1, 0), // Diagonal (up-right, down-left)
            new Vector3(1, -1, 0), new Vector3(-1, 1, 0) // Diagonal (up-left, down-right)
        };

        foreach (Vector3 dir in directions)
        {
            Vector3 adjacentPosition = block.transform.position + dir;

            // Check if there's a block at the adjacent position
            Collider[] colliders = Physics.OverlapSphere(adjacentPosition, 0.2f); // Increase the radius to ensure accurate detection
            foreach (Collider collider in colliders)
            {
                Block adjacentBlock = collider.GetComponent<Block>();
                if (adjacentBlock != null && !linkedBlocks.Contains(adjacentBlock) && adjacentBlock.tag == block.tag)
                {
                    AddToLink(adjacentBlock);
                }
            }
        }
    }

    public void EndLinking()
    {
        isLinking = false;

        if (selectedGameObjects.Count >= 3 && CheckBlocksMatch())
        {
            DestroySelectedBlocks();
            movescript.DecreaseMoveCount();
            StartCoroutine(waitandspawnblocks());
        }
        else
        {
            ResetLinkedBlocks();
        }

        selectedGameObjects.Clear(); // Clear the selected blocks for the next selection
        ResetHighlight();
    }

    public IEnumerator waitandspawnblocks()
    {
        yield return new WaitForSeconds(1.0f);
        spawner.SpawnLineOfBlocks();
    }

    private void HighlightBlock(Block block)
    {
        if (block != null)
        {
            block.GetComponent<Renderer>().material.color = Color.gray; // Highlight the block
        }
    }

    private void ResetHighlight()
    {
        foreach (var block in linkedBlocks)
        {
            if (block != null)
            {
                block.GetComponent<Renderer>().material.color = Color.white; // Reset color
            }
        }
    }

    private bool CheckBlocksMatch()
    {
        if (selectedGameObjects.Count == 0) return false;

        string firstTag = selectedGameObjects[0].tag;
        foreach (GameObject block in selectedGameObjects)
        {
            if (block.tag != firstTag)
            {
                return false;
            }
        }
        return true;
    }

    private void DestroySelectedBlocks()
    {
        foreach (GameObject blockObject in selectedGameObjects)
        {
            Block block = blockObject.GetComponent<Block>();
            if (block != null && linkedBlocks.Contains(block)) // Ensure the block is still linked
            {
                Destroy(blockObject);
                linkedBlocks.Remove(block); // Remove the block from the linked blocks list
            }
        }
    }

    private void ResetLinkedBlocks()
    {
        for (int i = linkedBlocks.Count - 1; i >= 0; i--)
        {
            Block block = linkedBlocks[i];
            if (block != null)
            {
                block.isLinked = false;
                block.IsSelected = false; // Ensure isSelected is also reset
                block.GetComponent<Renderer>().material.color = Color.white; // Reset color
            }
            linkedBlocks.RemoveAt(i);
        }
    }
}
