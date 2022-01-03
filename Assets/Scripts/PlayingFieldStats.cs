using UnityEngine;
public sealed class PlayingFieldStats
{
    int _chainLength;
    public int ChainLength {
        get
        {
            return _chainLength;
        }
        set
        {
            _chainLength = value;

            if (_chainLength > MaxChainLength)
            {
                MaxChainLength = _chainLength;
                PlayingField.PostMaxChain();
            }
        }
    }
    public int MaxChainLength { get;private set;}
    public int PreviousLevel { get; private set; }
    public int Level { get; private set; }
    int _score;
    public int Score {
        get
        {
            return _score;
        }
        set
        {
            _score = value;

            if (_score > BestScore)
            {
                BestScore = _score;
                PlayingField.PostBest();
            }

            PlayingField.PostScore();
        }
    }
    public int BestScore { get; private set; }
    PlayingFieldStats()
    {
        ChainLength = 0;
        MaxChainLength = ChainLength;
        Level = 1;
        PreviousLevel = Level;
        Score = MaxChainLength;
        Debug.Log(GameManager.SelectedConfig.ID);
        BestScore = GameManager.PlayerModel.SoloBestScores[GameManager.SelectedConfig.ID];
    }
    public static PlayingFieldStats CreateNew()
    {
        return new PlayingFieldStats();
    }
    public void CheckLevel()
    {
        Level = Mathf.FloorToInt(_score / 1000) + 1;

        if(PreviousLevel != Level)
        {
            PreviousLevel = Level;
            PlayingField.PostLevel();
        }
    }
}
