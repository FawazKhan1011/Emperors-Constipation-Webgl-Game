using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    public bool isplaying = false;
    public AudioSource Victory;
    public Animator Emperor_animator;
    public bool isGrounded;
    public bool isgrunt = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Medicine"))
        {
            isGrounded = true;
            Debug.Log("isGrounded is set to"  + isGrounded);
            //Debug.Log("Medicine is Grounded");
            isgrunt = true;
            Emperor_animator.SetBool("OnGrounded", true);
            Victory.Play();
            isplaying = true;

        }
    }
}
