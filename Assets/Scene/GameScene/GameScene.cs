using _Scripts.Scene;

namespace Scene.GameScene
{
    public class GameScene : sceneBase
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
            GameManager.Instance.StartGame();
        }

        #endregion

        #region ----- BUTTON EVENT -----

        private void OnButtonSkillClick(int id)
        {
        }

        #endregion

        #region ----- PUBLIC FUNCTION -----
        
        public void RefreshUI()
        {
        }

        public void SkillPlayed(int id)
        {
        }

        #endregion
    }
}
