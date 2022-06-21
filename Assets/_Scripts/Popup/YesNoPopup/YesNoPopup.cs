using System;
using _Prefab.Popup;
using _Scripts.qtLib;
using _Scripts.System;
using Scene.GameScene;
using TMPro;
using UnityEngine;

public class YesNoPopup : popBase
{
    [SerializeField] private qtButton btnYes;
    [SerializeField] private qtButton btnNo;
    
    private Action _evtYes;
    private Action _evtNo;
    protected override void OnEnable()
    {
        base.OnEnable();
        btnYes.onClick.AddListener(OnButtonYesClick);
        btnNo.onClick.AddListener(OnButtonNoClick);
    }

    public YesNoPopup Initialize(string content, Action yes = null, Action no = null)
    {
        _evtYes = yes;
        _evtNo = no;
        _txtContent.text = content;
        return this;
    }

    private void OnButtonYesClick()
    {
        Hide();
        _evtYes?.Invoke();
    }

    private void OnButtonNoClick()
    {
        Hide();
        _evtNo?.Invoke();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        btnYes.onClick.RemoveAllListeners();
        btnNo.onClick.RemoveAllListeners();
        _evtNo = null;
        _evtYes = null;
    }
}
