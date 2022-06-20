using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class qtScene
{
    private static SceneData _sceneData;
    public static SceneData sceneData
    {
        get
        {
            if (_sceneData == null)
            {
                _sceneData = Resources.Load<SceneData>("_Data/SceneConfig");
            }

            return _sceneData;
        }
    }
    public enum EPopup
    {
        Noti = 0,
        YesNo = 1,
        ChangeInfo = 2,
        Setting = 3,
        ConfirmPlayGame = 4,
        MergeCard = 5,
        SplitCard = 6,
        Lose = 7,
        Win = 8,
        InGameSetting = 9,
    }

    public enum EScene
    {
        IntroScene = 0,
        MainMenu = 1,
        GameScene = 2
    }

    public enum EHud
    {
        Menu = 0
    }
}