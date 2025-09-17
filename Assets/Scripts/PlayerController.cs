using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private float baseSpeed;

    [Header("Shooting")]
    public GameObject pelletPrefab;
    public Transform firePoint;
    public float shootCooldown = 0.3f;
    private float lastShootTime = 0f;
    public AudioClip shootSFX;

    [Header("Sprites")]
    public Sprite frontSprite;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    [Header("Powerups")]
    private bool doubleShotEnabled = false;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Coroutine revertSpriteCoroutine;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        spriteRenderer.sprite = frontSprite;
        baseSpeed = moveSpeed; // Store original speed
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        // Movement input
        moveInput.x = 0;
        moveInput.y = 0;

        if (Input.GetKey(KeyCode.A)) moveInput.x = -1;
        if (Input.GetKey(KeyCode.D)) moveInput.x = 1;
        if (Input.GetKey(KeyCode.W)) moveInput.y = 1;
        if (Input.GetKey(KeyCode.S)) moveInput.y = -1;

        // Sprite flipping
        if (Input.GetKey(KeyCode.D))
        {
            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            spriteRenderer.flipX = true;
        }
    }

    private void HandleShooting()
    {
        Vector2 shootDirection = Vector2.zero;

        if (Time.time - lastShootTime >= shootCooldown)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) shootDirection = Vector2.up;
            else if (Input.GetKeyDown(KeyCode.DownArrow)) shootDirection = Vector2.down;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) shootDirection = Vector2.left;
            else if (Input.GetKeyDown(KeyCode.RightArrow)) shootDirection = Vector2.right;

            if (shootDirection != Vector2.zero)
            {
                Shoot(shootDirection);
                lastShootTime = Time.time;
            }
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    void Shoot(Vector2 dir)
    {
        // Primary shot
        CreatePellet(transform.position, dir);
        
        // Double shot (if enabled)
        if (doubleShotEnabled)
        {
            Vector3 offset = Vector3.zero;
            
            // Create offset perpendicular to shooting direction
            if (dir == Vector2.up || dir == Vector2.down)
            {
                offset = new Vector3(0.5f, 0, 0); // Horizontal offset for vertical shots
            }
            else
            {
                offset = new Vector3(0, 0.5f, 0); // Vertical offset for horizontal shots
            }
            
            // Create second pellet with offset
            CreatePellet(transform.position + offset, dir);
        }
        
        SetShootingSprite(dir);
        
        if (shootSFX != null && audioSource != null)
            audioSource.PlayOneShot(shootSFX);
    }

    private void CreatePellet(Vector3 position, Vector2 direction)
    {
        GameObject pellet = Instantiate(pelletPrefab, position, Quaternion.identity);
        Pellet pelletScript = pellet.GetComponent<Pellet>();
        if (pelletScript != null)
        {
            pelletScript.direction = direction;
        }
    }

    void SetShootingSprite(Vector2 dir)
    {
        if (revertSpriteCoroutine != null)
            StopCoroutine(revertSpriteCoroutine);

        if (dir == Vector2.up)
        {
            spriteRenderer.sprite = upSprite;
            spriteRenderer.flipX = false;
        }
        else if (dir == Vector2.down)
        {
            spriteRenderer.sprite = downSprite;
            spriteRenderer.flipX = false;
        }
        else if (dir == Vector2.left)
        {
            spriteRenderer.sprite = leftSprite;
            spriteRenderer.flipX = true;
        }
        else if (dir == Vector2.right)
        {
            spriteRenderer.sprite = leftSprite;
            spriteRenderer.flipX = false;
        }

        revertSpriteCoroutine = StartCoroutine(RevertToFrontSprite());
    }

    IEnumerator RevertToFrontSprite()
    {
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.sprite = frontSprite;
    }

    // Powerup Methods
    public void EnableDoubleShot()
    {
        doubleShotEnabled = true;
        Debug.Log("Double Shot Enabled!");
    }

    public void IncreaseSpeed()
    {
        moveSpeed += 2f;
        Debug.Log($"Speed increased! New speed: {moveSpeed}");
    }

    public void IncreaseHealth(int amount)
    {
        PlayerHealth healthComponent = GetComponent<PlayerHealth>();
        if (healthComponent != null)
        {
            healthComponent.maxHealth += amount;
            healthComponent.Heal(amount); // Also heal current health
            Debug.Log($"Max health increased by {amount}!");
        }
    }

    // Reset powerups (called when starting new run)
    public void ResetPowerups()
    {
        doubleShotEnabled = false;
        moveSpeed = baseSpeed;
        Debug.Log("Player powerups reset");
    }
}