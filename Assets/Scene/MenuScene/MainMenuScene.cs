using System.Collections.Generic;
using _Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Scene.MainMenuScene
{
    public class MainMenuScene : sceneBase
    {
        #region ----- VARIABLE -----

        
        #endregion
        
        #region ----- INITIALIZE -----

        public override void Initialize()
        {
            base.Initialize();

        }
        
        public override void InitObject()
        {
        }
        
        protected override void InitEvent()
        {
        }

        public override void RemoveEvent()
        {
        }

        #endregion

        #region ----- ANIMATION -----

        public override void Show()
        {
            UIManager.Instance.ignoreCast = false;
        }

        public override void Hide()
        {
        }

        #endregion

        #region ----- BUTTON EVENT -----

        public void OnButtonHomeClick()
        {
        }

        #endregion

        #region ----- PUBLIC FUNCTION -----


        #endregion

        #region ----- PRIVATE FUNCTION -----



        #endregion
    }
}
