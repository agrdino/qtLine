using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int best;
    public bool isSaving;
    public int score;
    public float playTime;
    public List<Ball> balls;

    public PlayerData()
    {
        balls = new List<Ball>();
        isSaving = false;
    }
}

[Serializable]
public class Ball
{
    public int col;
    public int row;
    public int color;
    public EBallType type;
    public EBallState state;
}

[Serializable]
public class Level
{
    public string name;
    public bool undoAllow;
    public int startBalls;
    public int queueBalls;
    public bool hasBonusBall;
    public int ratioActiveBonusBall;

    public Level()
    {
        name = "Default level";
    }

    public void CreateTempLevel()
    {
        undoAllow = true;
        startBalls = 8;
        queueBalls = 3;
        hasBonusBall = true;
        ratioActiveBonusBall = 20;
    }
}

[Serializable]
public class LevelData
{
    public List<Level> levelData;

    public LevelData()
    {
        levelData = new List<Level>();
    }
}


[Serializable]
public class Step
{
    public List<Ball> balls;
    public int score;

    public Step()
    {
        balls = new List<Ball>();
    }
}

public enum EBallState
{
    Normal = 0,
    Queue = 1,
}

public enum EBallType
{
    Normal = 0,
    Ghost = 1
}
