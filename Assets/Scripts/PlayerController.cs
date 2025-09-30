using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 0;

    [Header("UI")]
    public TextMeshProUGUI countText;
    public GameObject winTextObject;   // Your existing "WinText" TMP object (used for both win/lose)

    [Header("Gameplay")]
    [Tooltip("Leave 0 to auto-detect from objects tagged 'PickUp' at Start.")]
    public int totalPickups = 0;

    [Header("Managers")]
    public GameManager gameManager;    // Drag the GameManager (with end menu) here

    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    private bool isGameOver;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        isGameOver = false;

        // Auto-detect number of pickups if not set in Inspector
        if (totalPickups <= 0)
        {
            GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
            totalPickups = (pickups != null && pickups.Length > 0) ? pickups.Length : 12; // fallback for tutorial
        }

        SetCountText();

        if (winTextObject != null)
            winTextObject.SetActive(false);
        else
            Debug.LogWarning("winTextObject is not assigned in the Inspector!");
    }

    void OnMove(InputValue movementValue)
    {
        if (isGameOver) return;

        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText()
    {
        if (countText == null)
        {
            Debug.LogError("Count Text is not assigned in the Inspector!");
            return;
        }

        countText.text = "Score: " + count.ToString();

        // Win condition
        if (!isGameOver && count >= totalPickups)
        {
            EndGame("You Win!");
        }
    }

    void FixedUpdate()
    {
        if (isGameOver) return;

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGameOver) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Lose condition
            EndGame("You Lose!");

            // If you truly want to destroy the ball on lose, uncomment:
            // Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;

        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    private void EndGame(string message)
    {
        isGameOver = true;

        // Freeze player motion
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        movementX = 0f;
        movementY = 0f;

        // Update your existing on-screen TMP text
        if (winTextObject != null)
        {
            var tmp = winTextObject.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = message;
            winTextObject.SetActive(true);
        }

        // Show end menu (Replay only)
        if (gameManager != null)
        {
            gameManager.ShowEndMenu(message);
        }
        else
        {
            Debug.LogWarning("GameManager is not assigned on PlayerController.");
        }
    }
}
