using Photon.Pun;

public class PlayerModelClientConnection : MonoBehaviourPun
{
    public string playerName;
    public int playerID = 0;
    public bool playerIsAlive = false;
    public int currentScore = 0;

    public void SetID(int pID)
    {
        playerID = pID;
        playerIsAlive = true;
        SetName();
    }

    public void SetName()
    {

        playerName = GameManager.PlayerModel.UserName;
    }
}
