using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    // You could change from Transform to Rigidbody2D for a physics-based implementation (e.g. sliding)
    private Transform player;
    [Header("Player Components")]
    [SerializeField] private Animator animator;
    [SerializeField] GameObject playerTag;
    public bool hasEarpiece = false;
    public delegate void PlayerState();
    [HideInInspector] public PlayerState currentState;

    // Examples of a player attribute (should be used put a Scriptable Object (data container) (e,g. PlayerStats.speed)
    [Header("Player Stats")]
    [SerializeField, Range(0f, 20f)] float speed = 2f;
    [SerializeField, Range(0f, 1f)] float slideTime = 0.5f;
    float slideCooldown = 0f;

    private void Start()
    {
        player = GetComponent<Transform>();
        currentState = Idle;
    }

    private void Update()
    {
        // If this isn't the player's character, don't control this player (regarding network)
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        // Run current state
        currentState.Invoke();
    }


    // States

    public void Moving()
    {
        // Handle Movement
        CommandMove(speed);

        animator.Play("Run");

        // State transition for Sliding
        if (!(slideCooldown > slideTime)) slideCooldown += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && slideCooldown > slideTime) currentState = Rolling;
    }

    public void Rolling()
    {
        // Handle Increased Movement
        CommandMove(speed * 2);

        animator.Play("Roll");

        // State transition for back to Moving
        if (slideCooldown <= 0f) currentState = Moving;
        else slideCooldown -= Time.deltaTime;
    }

    public void Waiting()
    {
        // for if we wait for the round or something that requires the player to not move
        animator.Play("Idle");
        // Debug.Log("Player is waiting...");
    }

    public void Idle()
    {
        animator.Play("Idle");

        // if the player is moving again
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            currentState = Moving;
        }
    }

    public async void Loser()
    {
        // for if the player is out of the game
        animator.Play("Death");

        // Wait for the animation to finish
        await Task.Delay(1200);

        // Remove player
        Destroy(gameObject);

        // Debug.Log("Player is out of the game...");
    }


    public void Winner()
    {
        // for if the player is the last one standing
        // dance animation
        Debug.Log("Player is the last one standing...");
    }


    // Helper functions

    // Command Pattern for Handling Input = Movement
    private void CommandMove(float speedMult)
    {
        float moveX = player.position.x;
        float moveY = player.position.y;

        // flip player based on direction
        if (Input.GetKey(KeyCode.D))
        {
            player.localRotation = Quaternion.Euler(0, 0, 0);
            playerTag.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            player.localRotation = Quaternion.Euler(0, 180, 0);
            playerTag.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        // Update player's position based on direction of input
        moveX += Input.GetAxis("Horizontal") * speedMult * Time.deltaTime * 2;
        moveY += Input.GetAxis("Vertical") * speedMult * Time.deltaTime * 2;

        player.position = new Vector2(moveX, moveY);

        // Whatever State it is called in, if there is no input, then switch to idle state
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            currentState = Idle;
        }
    }
}
