using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace _Scripts.Handler
{
    public class BallHandler : MonoBehaviour
    {
        public int color
        {
            get;
            private set;
        }

        private Image _imgBall;
        private bool _isInit;
        private SortingGroup _sortingGroup;

        public EBallState ballState
        {
            get;
            private set;
        }
        
        private void InitObject()
        {
            if (!_isInit)
            {
                _isInit = true;
                _imgBall = GetComponent<Image>();
                _sortingGroup = GetComponent<SortingGroup>();
            }
        }

        public void Initialize(int color, EBallState state)
        {
            InitObject();
            ballState = state;
            this.color = color;
            _imgBall.color = DataManager.Instance.colorBank[color];
        }

        public void Move(Vector3[] movePath)
        {
            //Todo: Move
        }

        public void Grow()
        {
            ballState = EBallState.Normal;
            transform.DOScale(0.9f * Vector3.one, 0.25f);
        }
    }
}
