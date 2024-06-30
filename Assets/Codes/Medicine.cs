
using UnityEngine;

public class Medicine : MonoBehaviour
{
    public Animation door_animation;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Emperor"))
        {
            //Debug.Log("Emperor Collided");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            //Debug.Log("Object Collided");
        }
    }
}
