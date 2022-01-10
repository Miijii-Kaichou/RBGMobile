using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameWatcher : MonoBehaviourPun
{

    private static GameWatcher Instance;
    public PlayerModelClientConnection me => playerMMCs[0];

    [SerializeField]
    GameObject[] playerObjs = new GameObject[2];

    [SerializeField]
    PlayerModelClientConnection[] playerMMCs = new PlayerModelClientConnection[2];


    private void Awake()
    {
        Instance = this;
    }

    public void Init()
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

    public static void AddToPlayerList(GameObject obj, PlayerModelClientConnection pmcc, int id)
    {
        Instance.playerObjs[id] = obj;
        Instance.playerMMCs[id] = pmcc;
    }

    void InitilizeNetworkedInfo()
    {
        int player = 0;
        if (!PhotonNetwork.IsMasterClient)
        {
            player = 1;
        }

        AddNewPlayer(player);
    }


    public void AddNewPlayer(int player)
    {

        playerObjs[player] = PhotonNetwork.InstantiateRoomObject("PlayerMCC", transform.position, Quaternion.identity);
        PlayerModelClientConnection pmcc = playerObjs[player].GetComponent<PlayerModelClientConnection>();
        pmcc.SetID(player);
        pmcc.SetName();
        playerMMCs[player] = pmcc;
    }

    public PlayerModelClientConnection[] GetPMCC() => playerMMCs;
}