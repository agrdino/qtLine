using System;
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
    
    #endregion
    
    #region ----- PUBLIC FUNCTION -----
    public void SaveData()
    {
        // if (playerData != null)
        // {
        //     string data = JsonUtility.ToJson(playerData);            
        //     File.WriteAllText($"{DataPath}/PlayerData.dat", data);
        // }
    }

    #endregion

    #region ----- PRIVATE FUNCTION -----

    private void LoadData()
    {
        // if (File.Exists($"{DataPath}/PlayerData.dat"))
        // {
        //     playerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText($"{DataPath}/PlayerData.dat"));
        // }
        // else
        // {
        //     playerData = new PlayerData();
        //     SaveData();
        // }
    }

    #endregion
}
