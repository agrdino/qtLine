using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int best;
    public int time;
}

[Serializable]
public class Ball
{
    public int col;
    public int row;
    public int color;
    public int type;
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
