using UnityEngine;
using Photon.Pun;

public class GameWatcher : MonoBehaviourPun
{
    public PlayerModelClientConnection me => playerMMCs[0];

    [SerializeField]
    GameObject[] playerObjs = new GameObject[2];

    [SerializeField]
    PlayerModelClientConnection[] playerMMCs = new PlayerModelClientConnection[2];

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            InitilizeNetworkedInfo();
        }
    }

    public void BroadcastUpdateScore()
    {
        
    }

    public void BroadcastDefeat()
    {

    }

    void InitilizeNetworkedInfo()
    {
        int player = 0;
        if (!PhotonNetwork.IsMasterClient)
        {
            player = 1;
        }

        photonView.RPC("AddNewPlayer", RpcTarget.MasterClient, player);
        playerMMCs[player] = playerObjs[player].GetComponent<PlayerModelClientConnection>();
        playerMMCs[player].photonView.RPC("SetID", PhotonNetwork.LocalPlayer, player);
        playerMMCs[player].photonView.RPC("SetName", PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void AddNewPlayer(int player)
    {
        playerObjs[player] = PhotonNetwork.Instantiate("PlayerMCC", transform.position, Quaternion.identity);
    }

    public PlayerModelClientConnection[] GetPMCC() => playerMMCs;
}