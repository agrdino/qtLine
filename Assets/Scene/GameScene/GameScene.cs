using System.Collections.Generic;
using _Scripts.Scene;
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

        private float _time;
        
        #endregion

        #region ----- INITIALIZE -----

        public override void Initialize()
        {
            base.Initialize();
            _txtScore.text = "00";
        }

        public override void InitObject()
        {
            _txtTime = FindObjectWithPath(gameObject, "TopPanel/imgTimeHolder/txtTime").GetComponent<TextMeshProUGUI>();
            _txtScore = FindObjectWithPath(gameObject, "TopPanel/imgScoreHolder/txtScore").GetComponent<TextMeshProUGUI>();
            _txtHighScore = FindObjectWithPath(gameObject, "TopPanel/imgBestHolder/txtBest").GetComponent<TextMeshProUGUI>();
        }

        protected override void InitEvent()
        {
        }

        public override void RemoveEvent()
        {
        }

        #endregion

        private void Update()
        {
            var tempTime = Time.time - _time;
            _txtTime.text = $"{(int)(tempTime/60) : 00} :{(int)(tempTime % 60) : 00}";
        }

        #region ----- ANIMATION -----

        public override void Show()
        {
            GameManager.Instance.StartGame();
            _time = Time.time;
        }

        #endregion

        #region ----- BUTTON EVENT -----

        private void OnButtonSkillClick(int id)
        {
        }

        #endregion

        #region ----- PUBLIC FUNCTION -----
        
        public void UpdateUI()
        {
            _txtScore.text = GameManager.Instance.score.ToString();
        }

        public void SkillPlayed(int id)
        {
        }

        #endregion
    }
}
