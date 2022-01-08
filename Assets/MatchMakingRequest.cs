using UnityEngine;
using UnityEngine.UI;

public class MatchMakingRequest : MonoBehaviour
{
    [SerializeField]
    Button cancelButton;

    private void OnEnable()
    {
        BeginMatchMaking();
    }

    /// <summary>
    /// Will cancel the active process of looking for a match.
    /// </summary>
    public void CancelMatchMatching()
    {
        GameSessionLauncher.CancelMatchMaking();
    }

    /// <summary>
    /// Begin to search for a player to play against.
    /// </summary>
    void BeginMatchMaking()
    {
        GameSessionLauncher.RequestMatchMaking();

#if UNITY_EDITOR
        Debug.Log("Matchmaking..."); 
#endif
    }
}
