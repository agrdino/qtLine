using _Scripts.qtLib;
using _Scripts.System;
using System;
using TMPro;
using UnityEngine;

namespace _Prefab.Popup.YesNoPopup
{
    public class LosePopup : popBase
    {
        [SerializeField] private qtButton _btnYes;
        [SerializeField] private qtButton _btnNo;
        [SerializeField] private TextMeshProUGUI _txtScore;
        [SerializeField] private TextMeshProUGUI _txtHighScore;
    
        private Action _evtYes;
        private Action _evtNo;
        protected override void OnEnable()
        {
            base.OnEnable();
            _btnYes.onClick.AddListener(OnButtonYesClick);
            _btnNo.onClick.AddListener(OnButtonNoClick);
        }

        public LosePopup Initialize(Action yes = null, Action no = null)
        {
            _evtYes = yes;
            _evtNo = no;

            _txtScore.text = GameManager.Instance.score.ToString();
            _txtHighScore.text = "High score: " + DataManager.Instance.highScore;
            return this;
        }

        private void OnButtonYesClick()
        {
            _evtYes?.Invoke();
            GameManager.Instance.RestartGame();
            Hide();
        }

        private void OnButtonNoClick()
        {
            _evtNo.Invoke();
            GameManager.Instance.ClearGame();
            UIManager.Instance.ShowScene(qtScene.EScene.MainMenu);
            Hide();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _btnYes.onClick.RemoveAllListeners();
            _btnNo.onClick.RemoveAllListeners();
            _evtYes = null;
            _evtNo = null;
        }
    }
}