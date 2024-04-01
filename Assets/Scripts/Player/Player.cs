using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    // You could change from Transform to Rigidbody2D for a physics-based implementation (e.g. sliding)
    [Header("Player Components")]
    private Transform player;
    [SerializeField] private Animator animator;
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
        currentState = Moving;
    }

    private void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        currentState.Invoke();
    }

    // States
    public void Moving()
    {

        float moveX = player.position.x;
        float moveY = player.position.y;

        moveX += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        moveY += Input.GetAxis("Vertical") * speed * Time.deltaTime;

        // if not moving, then the player is idle
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            currentState = Idle;
        }


        player.position = new Vector2(moveX, moveY);

        // State transition for Sliding
        if (!(slideCooldown > slideTime)) slideCooldown += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && slideCooldown > slideTime) currentState = Sliding;
    }

    void Sliding()
    {
        float moveX = player.position.x;
        float moveY = player.position.y;

        moveX += Input.GetAxis("Horizontal") * speed * Time.deltaTime * 2;
        moveY += Input.GetAxis("Vertical") * speed * Time.deltaTime * 2;

        player.position = new Vector2(moveX, moveY);

        // State transition for back to Moving
        if (slideCooldown <= 0f) currentState = Moving;
        else slideCooldown -= Time.deltaTime;
    }

    public void Waiting()
    {
        // for if we wait for the round or something that requires the player to not move
        // Debug.Log("Player is waiting...");
    }

    public void Idle()
    {
        // if the player isn't moving
        // Debug.Log("Player is waiting...");
    }

    public void Loser()
    {
        // for if the player is out of the game
        // O(n) and delete childern of the player once animation (!isPlaying)
        // Debug.Log("Player is out of the game...");
        // Remove player
        // Destroy(gameObject);
    }

    public void Winner()
    {
        // for if the player is the last one standing
        // dance animation
        // Debug.Log("Player is the last one standing...");
    }
}
