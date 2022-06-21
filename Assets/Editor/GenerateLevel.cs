using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GenerateLevel : EditorWindow
{
    private static LevelData _levelData;

    [MenuItem("Tools/Game tool/Generate level")]
    public static void CreateWindow()
    {
        GenerateLevel window = (GenerateLevel) GetWindow(typeof(GenerateLevel));
        window.autoRepaintOnSceneChange = true;

        _selectedLevel = 0;
    }
    
    private Vector2 _levelEditorScrollV2;
    private Vector2 _levelScrollV2;
    private Vector2 _listLevelScrollV2;
    private Vector2 _contentScrollV2;
    private Level _tempLevel;

    private int _tempStartBall;
    private int _tempQueueBall;

    private static int _selectedLevel;

    private void OnGUI()
    {
        if (_levelData == null)
        {
            if (File.Exists(Application.dataPath + "/Resources/_Data/LevelData.txt"))
            {
                _levelData = JsonUtility.FromJson<LevelData>(Resources.Load<TextAsset>("_Data/LevelData").text);
            }
            else
            {
                _levelData = new LevelData();
            }
        }
        if (_tempLevel == null && _levelData.levelData.Count == 0)
        {
            _tempLevel = new Level();
            _tempLevel.CreateTempLevel();
            _levelData.levelData.Add(_tempLevel);
        }
        else if (_tempLevel == null)
        {
            _tempLevel = _levelData.levelData[0];
        }
        
        _contentScrollV2 = EditorGUILayout.BeginScrollView(_contentScrollV2, GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));
        {
            //CONTENT
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                DrawLevelList();
                DrawEditor();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Save data"))
        {
            File.WriteAllText(Application.dataPath + "/Resources/_Data/LevelData.txt", JsonUtility.ToJson(_levelData));
        }
    }
    
    private void DrawLevelList()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);

        //MAP LIST
        EditorGUILayout.BeginVertical("Box", GUILayout.Width(100));
        {
            _listLevelScrollV2 = EditorGUILayout.BeginScrollView(_listLevelScrollV2, false, false);
            {
                for (int i = 0; i < _levelData.levelData.Count; i++)
                {
                    if (i == _selectedLevel)
                    {
                        style.normal.textColor = Color.red;
                    }
                    else
                    {
                        style.normal.textColor = Color.white;
                    }
                    if (GUILayout.Button($"{_levelData.levelData[i].name}", style))
                    {
                        _tempLevel = _levelData.levelData[i];
                        _selectedLevel = i;
                    }
                }
            }

            if (GUILayout.Button("New level"))
            {
                var newLevel = new Level();
                _levelData.levelData.Add(newLevel);
                newLevel.name = $"Level {_levelData.levelData.IndexOf(newLevel)}";
            }

            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }
    
    private void DrawEditor()
    {
        //EDITOR CONTENT
        if (_levelData.levelData.Count <= 0)
        {
            EditorGUILayout.EndHorizontal();
            return;
        }

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(500));
                {
                    EditorGUILayout.BeginVertical();
                    {
                        GUILayout.Label("Level data:");
                        GUILayout.BeginVertical("Box");
                        {
                            int maxValue = Mathf.Clamp(_tempLevel.name.Length, 0, 20);
                            _tempLevel.name = EditorGUILayout.TextField("\tName: ", _tempLevel.name, GUILayout.Width(400))[..maxValue];
                            _tempLevel.undoAllow = EditorGUILayout.Toggle("\tAllow undo: ", _tempLevel.undoAllow, GUILayout.Width(250));
                            GUILayout.Space(10);
                            _tempLevel.startBalls = EditorGUILayout.IntField("\tStart with X balls: ", _tempLevel.startBalls, GUILayout.Width(250));
                            _tempLevel.startBalls = Mathf.Clamp(_tempLevel.startBalls, 1, 20);
                            _tempLevel.queueBalls = EditorGUILayout.IntField("\tQueue has X balls: ", _tempLevel.queueBalls, GUILayout.Width(250));
                            _tempLevel.queueBalls = Mathf.Clamp(_tempLevel.queueBalls, 1, 10);
                            GUILayout.Space(10);
                            _tempLevel.hasBonusBall = EditorGUILayout.Toggle("\tHas bonus ball: ", _tempLevel.hasBonusBall, GUILayout.Width(250));
                            if (_tempLevel.hasBonusBall)
                            {
                                _tempLevel.ratioActiveBonusBall = EditorGUILayout.IntField("\t% bonus ball: ", _tempLevel.ratioActiveBonusBall, GUILayout.Width(250));
                                _tempLevel.ratioActiveBonusBall = Mathf.Clamp(_tempLevel.ratioActiveBonusBall, 0, 100);
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Delete"))
            {
                int tempIndex = _levelData.levelData.IndexOf(_tempLevel);
                if (tempIndex >= 0)
                {
                    if (_levelData.levelData.Remove(_tempLevel))
                    {
                        if (_levelData.levelData.Count != 0)
                        {
                            if (tempIndex == _levelData.levelData.Count)
                            {
                                _tempLevel = _levelData.levelData[tempIndex - 1];
                            }
                            else
                            {
                                _tempLevel = _levelData.levelData[tempIndex];
                            }
                        }
                    }
                }
            }
        }
    }

}
