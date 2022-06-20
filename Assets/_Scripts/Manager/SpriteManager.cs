using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : qtSingleton<SpriteManager>
{
    public Sprite btnLock;
    public Sprite btnUnlock;

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject);
    }
}
