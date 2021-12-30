using UnityEngine;

/// <summary>
/// Gateway class will check if the user is a First-Time Player.
/// If they are, they will be sent to the Privacy and Agreements scene, then
/// to the Username scene to create a username.
/// 
/// If they are not a first time player, the player is logged in, and is sent to the Mainmenu
/// </summary>
public class Gateway : MonoBehaviour
{
    private void Start()
    {
        GameSceneManager.PrepareToLoad(1);
        LoadPlayerData();
        GameSceneManager.Deploy();
    }

    public void LoadPlayerData()
    {

    }
}