using UnityEngine;

public class DataManager : qtSingleton<DataManager>
{
    #region ----- VARIABLE -----
    
    private static string DataPath;
    //Color
    public Color32[] colorBank;
    
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
