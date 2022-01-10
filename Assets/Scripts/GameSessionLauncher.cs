using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum PlayMode
{
    Solo = 4,
    Crusades = 5,
    Verses = 6
}

public class GameSessionLauncher : MonoBehaviourPunCallbacks
{
    private static GameSessionLauncher Instance;

    public PlayMode SelectedPlayMode = PlayMode.Solo;

    [SerializeField]
    TextMeshProUGUI sceneTitleTMP;

    [SerializeField]
    TextMeshProUGUI[] buttonTextTMP;

    [SerializeField]
    GameObject[] playModeOverlays = new GameObject[2];

    Hashtable playerInfo = new Hashtable();


    string[] titles = new string[]
    {
        "Solo Play",
        "Crusades",
        "Verses"
    };

    string[] buttonTexts = new string[]
    {
        "Begin",
        "Find Match",
        "Search"
    };

    private void Awake()
    {
        Instance = this;
        SelectedPlayMode = GameManager.CurrentPlayMode;
    }

    private void Start()
    {
        Debug.Log(SelectedPlayMode);
        if (SelectedPlayMode != PlayMode.Solo)
            WarmUpPhotonNetwork();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        GameSceneManager.PrepareToLoad
            (SelectedPlayMode == PlayMode.Solo ?
            DefinedScenes.Game :
            DefinedScenes.GameNetworked
        );

        if(sceneTitleTMP != null && titles!= null)
        sceneTitleTMP.text = titles[(int)SelectedPlayMode - (int)PlayMode.Solo];

        if (buttonTextTMP.Length > 0)
        {
            for (int i = 0; i < buttonTextTMP.Length; i++)
                buttonTextTMP[i].text = buttonTexts[(int)SelectedPlayMode - (int)PlayMode.Solo];
        }
    }

    /// <summary>
    /// Enable an overlay
    /// </summary>
    /// <param name="overlayId"></param>
    public void EnableOverlay(int overlayId)
    {
        if (overlayId > 1) return;
        playModeOverlays[overlayId].SetActive(true);
    }

    /// <summary>
    /// Disable an overlay
    /// </summary>
    /// <param name="overlayId"></param>
    public void DisableOverlay(int overlayId)
    {
        if (overlayId > 1) return;
        playModeOverlays[overlayId].SetActive(false);
    }

    /// <summary>
    /// Set the play style for the game
    /// </summary>
    /// <param name="playStyle"></param>
    public void SetPlayStyle(int playStyle)
    {
        GameManager.SetPlayStyle((PlayStyle)playStyle);
    }

    public void LaunchGame(int difficulty)
    {
        GameManager.SetFPS(FrameRate.FPS60);

        GameManager.SetDiffConfig(difficulty);

        switch (SelectedPlayMode)
        {
            case PlayMode.Solo:
                GameManager.PlayerModel.TimesPlayedSolo++;
                GameSceneManager.Deploy();
                break;

            case PlayMode.Crusades:
                EnableOverlay(0);
                break;

            case PlayMode.Verses:
                EnableOverlay(1);
                break;

            default:
                break;
        }


        GameManager.PlayerModel.FirstTimeUser = false;
    }

    public void Back()
    {
        GameSceneManager.LoadPrevious();
        GameOverlay.EnableOverlay();
    }

    void WarmUpPhotonNetwork()
    {
        //Connect to the photon Server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public static void RequestMatchMaking()
    {
        //Try to join a pre existing room - if it fails, create one
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// Cancel trying to find a player
    /// </summary>
    public static void CancelMatchMaking()
    {
        Instance.DisableOverlay(0);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
#if UNITY_EDITOR
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + "/2 Starting Game");
#endif
            

            //TODO: Start game by configuring GameScene
            DisableOverlay(0);
            DisableOverlay(1);
            GameSceneManager.DeployAsNetworked();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        MakeRoom();
    }

    /// <summary>
    /// Make a room if match making fails.
    /// </summary>
    void MakeRoom()
    {
        int randomRoomName = (int)(System.DateTime.Now.Ticks - (long.MaxValue / 2));

        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2
        };

        PhotonNetwork.CreateRoom($"MatchEntity_{randomRoomName}", roomOptions);

#if UNITY_EDITOR
        Debug.Log($"MatchEntity_{randomRoomName}");
#endif
    }
}
