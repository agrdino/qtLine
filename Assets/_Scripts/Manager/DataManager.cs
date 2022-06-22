using System;
using System.IO;
using _Scripts.Handler;
using UnityEngine;

public class DataManager : qtSingleton<DataManager>
{
    #region ----- VARIABLE -----
    
    private static string DataPath;
    //Color
    public Color32[] colorBank;

    private GameObject _squareLightPrefab;
    public GameObject SquareLight
    {
        get
        {
            if (_squareLightPrefab == null)
            {
                _squareLightPrefab = Resources.Load<GameObject>("_Prefabs/squareLight");
            }

            return _squareLightPrefab;
        }
    }

    private GameObject _squareDarkPrefab;
    public GameObject SquareDark
    {
        get
        {
            if (_squareDarkPrefab == null)
            {
                _squareDarkPrefab = Resources.Load<GameObject>("_Prefabs/squareDark");
            }

            return _squareDarkPrefab;
        }
    }

    private GameObject _ballPrefab;
    public GameObject Ball
    {
        get
        {
            if (_ballPrefab == null)
            {
                _ballPrefab = Resources.Load<GameObject>("_Prefabs/ball");
            }

            return _ballPrefab;
        }
    }

    private GameObject _fxExplosive;

    public GameObject fxExplosive
    {
        get
        {
            if (_fxExplosive == null)
            {
                _fxExplosive = Resources.Load<GameObject>("_Prefabs/FX_Explosive");
            }

            return _fxExplosive;
        }
    }


    private PlayerData _playerData;
    public PlayerData playerData
    {
        get
        {
            if (_playerData == null)
            {
                LoadData();
            }

            return _playerData;
        }
    }

    private LevelData _levelData;

    public Level defaulLevel
    {
        get
        {
            if (_levelData == null)
            {
                _levelData = JsonUtility.FromJson<LevelData>(Resources.Load<TextAsset>("_Data/LevelData").text);
            }

            return _levelData.levelData[0];
        }
    }

    #endregion

    protected override void Init()
    {
        base.Init();
        DataPath = Application.persistentDataPath;
    }

    #region ----- PUBLIC FUNCTION -----
    public void SaveData(bool isSaveGame)
    {
        if (_playerData != null)
        {
            if (GameManager.Instance.IsEnd)
            {
                _playerData.best = GameManager.Instance.score >= _playerData.best
                    ? GameManager.Instance.score
                    : _playerData.best;
            }
            _playerData.balls.Clear();
            _playerData.isSaving = isSaveGame;
            if (isSaveGame)
            {
                _playerData.score = GameManager.Instance.score;
                _playerData.playTime = GameManager.Instance.playTime;
                foreach (var square in GameManager.Instance.squareForCheck)
                {
                    if (square.ball != null)
                    {
                        _playerData.balls.Add(new Ball()
                        {
                            col = square.col,
                            row = square.row,
                            color = square.ball.color,
                            state = square.ball.state,
                            type = square.ball.type
                        });
                    }
                }
            }
            string data = JsonUtility.ToJson(_playerData);    
            Debug.Log(data);
            File.WriteAllText($"{DataPath}/PlayerData.dat", data);
        }
    }

    #endregion

    #region ----- PRIVATE FUNCTION -----

    private void LoadData()
    {
        if (File.Exists($"{DataPath}/PlayerData.dat"))
        {
            _playerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText($"{DataPath}/PlayerData.dat"));
        }
        else
        {
            _playerData = new PlayerData();
            SaveData(false);
        }
    }

    #endregion
}
