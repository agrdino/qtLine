using System;
using System.Threading.Tasks;
using _Scripts.Scene;
using _Scripts.System;
using DG.Tweening;
using UnityEngine;

public class IntroScene : sceneBase
{
    private GameObject _logo;

    public override void Initialize()
    {
        base.Initialize();
        _logo.transform.position = new Vector3(0, - 2.5f, 0);
    }

    public override void InitObject()
    {
        _logo = qtHelper.FindObjectWithPath(gameObject, "MYSTIC LOGO");
    }
    
    protected override void InitEvent()
    {
    }

    public override void RemoveEvent()
    {
    }

    private void Update()
    {
    }

    public override void Show()
    {
        base.Show();
        transform.DOScale(Vector3.one, 5f).OnComplete(() =>
        {
            UIManager.Instance.ShowScene(qtScene.EScene.MainMenu);
        });
    }

    public override void Hide()
    {
        transform.DOScale(Vector3.one, 0.25f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
