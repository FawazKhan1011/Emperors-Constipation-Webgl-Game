using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isLinked = false;
    private bool isSelected = false; // Flag to prevent multiple triggers
    private bool isSpecialBlock = false; // Flag to check if the block is a special block

    public bool IsSelected
    {
        get => isSelected;
        set => isSelected = value;
    }

    public bool IsSpecialBlock
    {
        get => isSpecialBlock;
        set => isSpecialBlock = value;
    }

    void OnMouseDown()
    {
        if (!isSpecialBlock && !isSelected)
        {
            isSelected = true;
            if (!BlockLinker.instance.isLinking)
            {
                BlockLinker.instance.StartLinking(this);
               // Debug.Log($"Block Selected: {gameObject.name}, Tag: {gameObject.tag}");
            }
        }
    }

    void OnMouseEnter()
    {
        if (!isSpecialBlock && BlockLinker.instance.isLinking && !isLinked && !isSelected)
        {
            isSelected = true;
            BlockLinker.instance.AddToLink(this);
        }
    }

    void OnMouseUp()
    {
        if (BlockLinker.instance.isLinking)
        {
            BlockLinker.instance.EndLinking();
        }
        isSelected = false; // Reset the flag
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out hit))
            {
                Block hitBlock = hit.collider.GetComponent<Block>();

                if (hitBlock != null && !hitBlock.IsSelected && !hitBlock.IsSpecialBlock)
                {
                    hitBlock.IsSelected = true;
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            if (!BlockLinker.instance.isLinking)
                            {
                                BlockLinker.instance.StartLinking(hitBlock);
                               // Debug.Log($"Block Selected: {hitBlock.gameObject.name}, Tag: {hitBlock.gameObject.tag}");
                            }
                            break;
                        case TouchPhase.Moved:
                            if (BlockLinker.instance.isLinking && !hitBlock.isLinked)
                            {
                                BlockLinker.instance.AddToLink(hitBlock);
                            }
                            break;
                        case TouchPhase.Ended:
                            if (BlockLinker.instance.isLinking)
                            {
                                BlockLinker.instance.EndLinking();
                            }
                            break;
                    }
                    hitBlock.IsSelected = false; // Reset the flag
                }
            }
        }
    }
}
