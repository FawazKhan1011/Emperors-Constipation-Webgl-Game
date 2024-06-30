using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emperor : MonoBehaviour
{
    public AudioSource Victory;
    public float soundtimer = 5.0f; // Set this to your desired interval in seconds
    public AudioSource grunt;
    public float timer = 3.0f;
    public float speed;
    public float const_timer = 3.0f;
    public bool Finish = false;
    public Animator door_animator;
    public AudioSource Door;
    public Detection grunting;

    private SpriteRenderer emperorSpriteRenderer;
    private Animator Emperor_animator;
    private RectTransform rectTransform;
    private Coroutine gruntCoroutine;

    void Start()
    {
        emperorSpriteRenderer = GetComponent<SpriteRenderer>();
        rectTransform = GetComponent<RectTransform>();
        Emperor_animator = GetComponent<Animator>();
        gruntCoroutine = StartCoroutine(PlayGruntSound());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Medicine"))
        {
            if (!grunting.isplaying)
            {
                Victory.Play();
            }
            Door.Play();
            Debug.Log("Medicine Contact with Emperor");
            door_animator.SetBool("IsMedicineGrounded", true);
            Emperor_animator.SetBool("OnEmperor", true);
            Finish = true;
            Debug.Log("Finish is set to " + Finish);
            speed = Mathf.Abs(speed);
            emperorSpriteRenderer.flipX = false;
            if (gruntCoroutine != null)
            {
                StopCoroutine(gruntCoroutine);
            }
        }
    }

    void Update()
    {
        if (Finish)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                emperorSpriteRenderer.flipX = !emperorSpriteRenderer.flipX;
                timer = const_timer;
                speed = -speed;
            }
            transform.Translate(Vector3.right * speed * Time.deltaTime);

            // Restart coroutine if it has stopped
            if (gruntCoroutine == null && !Finish && !grunting.isgrunt)
            {
                gruntCoroutine = StartCoroutine(PlayGruntSound());
            }
        }
    }

    private IEnumerator PlayGruntSound()
    {
        while (!Finish && !grunting.isgrunt)
        {
            yield return new WaitForSeconds(soundtimer);
            if (!Finish && !grunting.isgrunt)
            {
                grunt.Play();   
            }
        }
        gruntCoroutine = null; // Reset the coroutine reference when it exits
    }
}
