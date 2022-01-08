using Photon.Pun;

public class PlayerModelClientConnection : MonoBehaviourPun
{
    public string playerName;
    public int playerID = 0;
    public bool playerIsAlive = false;
    public int currentScore = 0;

    [PunRPC]
    public void SetID(int pID)
    {
        playerID = pID;
        playerIsAlive = true;
    }

    [PunRPC]
    public void SetName()
    {
        if (photonView.IsMine)
            playerName = GameManager.PlayerModel.UserName;
    }
}
