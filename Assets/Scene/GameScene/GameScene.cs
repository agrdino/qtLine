using System.Collections.Generic;
using _Prefab.Popup.YesNoPopup;
using _Scripts.Handler;
using _Scripts.qtLib;
using _Scripts.Scene;
using _Scripts.System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static qtHelper;

namespace Scene.GameScene
{
    public class GameScene : sceneBase
    {
        #region ----- VARIABLE -----

        private TextMeshProUGUI _txtTime;
        private TextMeshProUGUI _txtScore;
        private TextMeshProUGUI _txtHighScore;

        private List<Image> _imgNextBalls;
        
        private Button _btnBackToMenu;
        private Button _btnUndo;
        private Button _btnRestart;

        private qtButton _btnBonusBall; 
        private Image _imgBonusBall; 

        private float _time;
        
        #endregion

        #region ----- INITIALIZE -----

        public override void Initialize()
        {
            base.Initialize();
            _txtScore.text = "00";
            _txtHighScore.text = DataManager.Instance.playerData.best.ToString();
        }

        public override void InitObject()
        {
            _txtTime = FindObjectWithPath(gameObject, "TopPanel/imgTimeHolder/txtTime").GetComponent<TextMeshProUGUI>();
            _txtScore = FindObjectWithPath(gameObject, "TopPanel/imgScoreHolder/txtScore").GetComponent<TextMeshProUGUI>();
            _txtHighScore = FindObjectWithPath(gameObject, "TopPanel/imgBestHolder/txtBest").GetComponent<TextMeshProUGUI>();

            _imgNextBalls ??= new List<Image>();
            var pnlBall = FindObjectWithPath(gameObject, "TopPanel/pnlNextBall/ballHolder");
            for (int i = 1; i < 4; i++)
            {
                _imgNextBalls.Add(FindObjectWithPath(pnlBall, $"imgBall{i}").GetComponent<Image>());
            }

            _btnBackToMenu = FindObjectWithPath(gameObject, "BotPanel/btnBack").GetComponent<qtButton>();
            _btnUndo = FindObjectWithPath(gameObject, "BotPanel/btnUndo").GetComponent<qtButton>();
            _btnRestart = FindObjectWithPath(gameObject, "BotPanel/btnRestart").GetComponent<qtButton>();
            _btnBonusBall = FindObjectWithPath(gameObject, "TopPanel/btnBonusBall").GetComponent<qtButton>();
            _imgBonusBall = FindObjectWithPath(gameObject, "TopPanel/btnBonusBall/imgBonusBall").GetComponent<Image>();
        }

        protected override void InitEvent()
        {
            _btnBackToMenu.onClick.AddListener(OnButtonBackClick);
            _btnUndo.onClick.AddListener(OnButtonUndoClick);
            _btnRestart.onClick.AddListener(OnButtonRestartClick);
            _btnBonusBall.onClick.AddListener(OnButtonBonusBallClick);
        }
        
        public override void RemoveEvent()
        {
            _btnRestart.onClick.RemoveAllListeners();
            _btnUndo.onClick.RemoveAllListeners();
            _btnBackToMenu.onClick.RemoveAllListeners();
            _btnBonusBall.onClick.RemoveAllListeners();
        }

        #endregion

        private void Update()
        {
            _txtTime.text = $"{(int)(GameManager.Instance.playTime/60) : 00} :{(int)(GameManager.Instance.playTime % 60) : 00}";
        }

        #region ----- ANIMATION -----

        public override void Show()
        {
            base.Show();
            _btnBonusBall.gameObject.SetActive(false);
            GameManager.Instance.StartGame();
            _time = Time.time;
        }

        public override void Hide()
        {
            base.Hide();
        }

        #endregion

        #region ----- BUTTON EVENT -----


        private void OnButtonRestartClick()
        {
            GameManager.Instance.RestartGame();
        }

        private void OnButtonUndoClick()
        {
            GameManager.Instance.Undo();
        }
        
        private void OnButtonBonusBallClick()
        {
            GameManager.Instance.BonusBall();
        }

        private void OnButtonBackClick()
        {
            ((YesNoPopup) UIManager.Instance.ShowPopup(qtScene.EPopup.YesNo)).Initialize(
                "Do you want to\nforfeit this game?\nYou can continue later",
                delegate
                {
                    GameManager.Instance.ClearGame(true);
                    UIManager.Instance.ShowScene(qtScene.EScene.MainMenu);
                });
        }

        
        #endregion

        #region ----- PUBLIC FUNCTION -----

        public void BonusBall(int color, bool show = true)
        {
            _btnBonusBall.DOKill();
            if (!show)
            {
                _btnBonusBall.transform.DOLocalMoveY(0, 0.5f).OnComplete(() => _btnBonusBall.gameObject.SetActive(false));
                return;
            }
            
            if (color == -1)
            {
                return;
            }

            _imgBonusBall.color = DataManager.Instance.colorBank[color];
            _btnBonusBall.transform.DOLocalMoveY(-200, 0.5f).OnStart(() =>
            {
                _btnBonusBall.gameObject.SetActive(true);
            });
        }
        
        public void UpdateUI()
        {
            _txtScore.text = GameManager.Instance.score.ToString();
        }

        public void UpdateQueue(List<SquareHandler> queue)
        {
            for (int i = 0; i < _imgNextBalls.Count; i++)
            {
                if (i >= queue.Count)
                {
                    _imgNextBalls[i].gameObject.SetActive(false);
                    continue;
                }
                _imgNextBalls[i].gameObject.SetActive(true);
                _imgNextBalls[i].color = DataManager.Instance.colorBank[queue[i].ball.color];
            }
        }
        
        #endregion
    }
}
