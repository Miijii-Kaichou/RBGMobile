using UnityEngine;
using Photon.Pun;

public class PlayerListDisplayNetworked : MonoBehaviourPun
{
    
    [SerializeField]
    public static GameWatcher networkHandler;

    [SerializeField]
    OpposingPlayerTag[] opTags = new OpposingPlayerTag[3];

    public void Start()
    {
        networkHandler = PhotonNetwork.InstantiateRoomObject("NetworkHandler", transform.position, Quaternion.identity).GetComponent<GameWatcher>();
        networkHandler.Init();
    }

    [PunRPC]
    public void SetUpPlayerTags()
    {
        PlayerModelClientConnection[] playerClients = networkHandler.GetPMCC();
        int tagIndex = 0;
        for(int i = 0; i < 4; i++)
        {
            if (!playerClients[i].playerID.Equals(networkHandler.me.playerID))
            {
                opTags[tagIndex].gameObject.SetActive(true);
                opTags[tagIndex].SetUp(playerClients[i], i);
                tagIndex++;
            }
        }
    }
}
