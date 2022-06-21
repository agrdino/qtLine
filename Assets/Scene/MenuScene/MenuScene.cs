using _Scripts.qtLib;
using _Scripts.Scene;
using _Scripts.System;
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
        }
        
        public override void InitObject()
        {
            _btnPlay = FindObjectWithPath(gameObject, "btnPlay").GetComponent<qtButton>();
            _btnContinue = FindObjectWithPath(gameObject, "btnPlay").GetComponent<qtButton>();
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
            UIManager.Instance.ShowScene(qtScene.EScene.GameScene);
        }

        private void OnButtonContinueClick()
        {
            //Todo
        }

        #endregion

        #region ----- PUBLIC FUNCTION -----


        #endregion

        #region ----- PRIVATE FUNCTION -----



        #endregion
    }
}
