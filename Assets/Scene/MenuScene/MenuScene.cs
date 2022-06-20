using _Scripts.Scene;
using _Scripts.System;

namespace Scene.MenuScene
{
    public class MenuScene : sceneBase
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
