using UnityEngine;
using Photon.Pun;

public class EarpiecePickup : MonoBehaviourPun
{
    // Pickup, destroy. Make sure Player doesnt have an earpiece already.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (photonView.IsMine && other.CompareTag("Player") && !other.GetComponent<Player>().hasEarpiece)
        {
            photonView.RPC("Pickup", RpcTarget.AllBuffered);
            other.GetComponent<Player>().hasEarpiece = true;
        }
    }

    // Sync the earpiece pickup being gone and now picked up onto the network.
    [PunRPC]
    void Pickup()
    {
        Destroy(this.gameObject);
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
