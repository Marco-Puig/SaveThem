using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    // You could change from Transform to Rigidbody2D for a physics-based implementation (e.g. sliding)
    private Transform player;

    private delegate void PlayerState();
    private PlayerState currentState;

    // Examples of a player attribute (should be used put a Scriptable Object (data container) (e,g. PlayerStats.speed)
    [Header ("Player Stats")]
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
    void Moving() 
    {
        float moveX = player.position.x;
        float moveY = player.position.y;

        moveX += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        moveY += Input.GetAxis("Vertical") * speed * Time.deltaTime;

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

    void Waiting()
    {
        // for if we wait for the round or something that requires the player to not move
        Debug.Log("Player is waiting...");
    }
    
    void Idle()
    {
        // add state transition from idle to move and vise versa
    }

    // These are what we give to our event in our eventsystem for the game manager singleton
    void WaitForRound()
    {
        currentState = Waiting;
    }
  
    void StartRound()
    {   
        currentState = Moving;
    }
}
