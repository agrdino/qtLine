using DG.Tweening;
using UnityEngine;
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

        public EBallState state
        {
            get;
            private set;
        }

        public EBallType type
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

        public void Initialize(int color, EBallState state, EBallType type = EBallType.Normal)
        {
            InitObject();
            this.state = state;
            this.color = color;
            _imgBall.color = DataManager.Instance.colorBank[color];
            this.type = type;
            transform.GetChild(0).gameObject.SetActive(type == EBallType.Ghost);
        }

        public void Move(Vector3[] movePath)
        {
            //Todo: Move
        }

        public void Grow()
        {
            state = EBallState.Normal;
            transform.DOScale(0.9f * Vector3.one, 0.25f);
        }
    }
}
