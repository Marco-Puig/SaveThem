using UnityEngine;
using Photon.Pun;

public class Pickup : MonoBehaviourPun
{
    // Pickup, destroy. Make sure Player doesnt have an earpiece already.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Player>().hasEarpiece)
        {
            PickupItem();
            other.GetComponent<Player>().hasEarpiece = true;
        }
    }

    // Pickup, destroy. After making sure Player doesnt have an earpiece already.
    void PickupItem()
    {
        Debug.Log("Picked up earpiece.");
        Destroy(gameObject);
    }

    /*
    // IPunObservable stuff:
    // TEST: Sync the isPickedUp variable across the network.
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to other players.
            stream.SendNext(isPickedUp);
        }
        else
        {
            // Receive data from the owner and update the variable.
            isPickedUp = (bool)stream.ReceiveNext();
        }
    }
    */
}
