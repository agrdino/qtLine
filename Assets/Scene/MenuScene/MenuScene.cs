using _Scripts.qtLib;
using _Scripts.Scene;
using _Scripts.System;
using UnityEngine;
using UnityEngine.UI;
using static qtHelper;

namespace Scene.MenuScene
{
    public class MenuScene : sceneBase
    {
        #region ----- VARIABLE -----

        private Button _btnPlay;
        private Button _btnContinue;

        #endregion
        
        #region ----- INITIALIZE -----

        public override void Initialize()
        {
            base.Initialize();
            _btnContinue.interactable = DataManager.Instance.playerData.isSaving;
        }
        
        public override void InitObject()
        {
            _btnPlay = FindObjectWithPath(gameObject, "btnPlay").GetComponent<qtButton>();
            _btnContinue = FindObjectWithPath(gameObject, "btnContinue").GetComponent<qtButton>();
        }
        
        protected override void InitEvent()
        {
            _btnContinue.onClick.AddListener(OnButtonContinueClick);
            _btnPlay.onClick.AddListener(OnButtonPlayClick);
        }
        
        public override void RemoveEvent()
        {
            _btnContinue.onClick.RemoveAllListeners();
            _btnPlay.onClick.RemoveAllListeners();
        }

        #endregion

        #region ----- ANIMATION -----
        
        #endregion

        #region ----- BUTTON EVENT -----

        private void OnButtonPlayClick()
        {
            DataManager.Instance.playerData.isSaving = false;
            UIManager.Instance.ShowScene(qtScene.EScene.GameScene);
        }

        private void OnButtonContinueClick()
        {
            UIManager.Instance.ShowScene(qtScene.EScene.GameScene);
        }

        #endregion

        #region ----- PUBLIC FUNCTION -----


        #endregion

        #region ----- PRIVATE FUNCTION -----



        #endregion
    }
}
