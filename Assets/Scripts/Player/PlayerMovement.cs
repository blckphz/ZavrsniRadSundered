using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Sprite normalSprite;
    public Sprite backSprite;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.y > 0)
        {

            sr.sprite = backSprite;
        }

        else if (moveInput.y < 0)
        {

            sr.sprite = normalSprite;
        }

    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
}