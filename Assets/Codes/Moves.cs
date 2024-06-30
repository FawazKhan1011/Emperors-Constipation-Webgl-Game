using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Moves : MonoBehaviour
{
    public Button shuffle;
    public Detection detectscript;
    public Emperor emperor_script;
    public TMP_Text move;
    private int movecount;
    // Start is called before the first frame update
    void Start()
    {
        movecount = 30;
        UpdateMoveText();
    }
    private void Update()
    {
        if (emperor_script.Finish)
        {
            StartCoroutine(WinCondition());
        }
    }
    public IEnumerator WinCondition()
    {
        Debug.Log("You Win");
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Win");
    } 

    public void DecreaseMoveCount()
    {
        if (movecount > 1)
        {
            movecount--;
            UpdateMoveText();
        }
        else if (movecount <= 1)
        {
            move.text = ("0");
            shuffle.enabled = false;
          StartCoroutine(Loosecondition());
        }
    }
    public IEnumerator Loosecondition()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("checking isGrounded" + detectscript.isGrounded);
        Debug.Log("checking Finish" + emperor_script.Finish);
        if (detectscript.isGrounded == false && emperor_script.Finish == false)
        {
            SceneManager.LoadScene("Loose");
        }
    }
    void UpdateMoveText()
    {
        move.text = movecount.ToString();
    }
}
