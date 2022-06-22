using _Scripts.System;
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
        private bool _isSelect;

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
            }
        }

        public void Initialize(int color, EBallState state, EBallType type = EBallType.Normal)
        {
            InitObject();
            transform.DOKill();
            _isSelect = false;
            this.state = state;
            if (state == EBallState.Queue)
            {
                transform.localScale = 0.3f * Vector3.one;
            }
            else
            {
                transform.localScale = 0.8f * Vector3.one;
            }
            this.color = color;
            _imgBall.color = DataManager.Instance.colorBank[color];
            this.type = type;
            transform.GetChild(0).gameObject.SetActive(type == EBallType.Ghost);
            _imgBall.transform.DOKill();
        }
        
        public void Grow()
        {
            state = EBallState.Normal;
            transform.DOScale(0.8f * Vector3.one, 0.25f);
        }

        public void Select()
        {
            _isSelect = !_isSelect;
            _imgBall.transform.DOKill();
            if (_isSelect)
            {
                AudioManager.Instance.PlaySfx("ball_sfx");
                _imgBall.transform.DOScale(0.9f * Vector3.one, 0.5f).SetEase(Ease.OutQuint)
                    .OnComplete(() =>
                    {
                        _imgBall.transform.DOScale(0.8f * Vector3.one, 0.5f)
                            .SetEase(Ease.OutQuint);
                    }).SetLoops(-1);
            }
            else
            {
                _imgBall.transform.DOScale(0.8f * Vector3.one, 0.25f);
            }
        }

        public void Score()
        {
            var fx = qtPooling.Instance.Spawn(DataManager.Instance.fxExplosive.name, DataManager.Instance.fxExplosive);
            fx.transform.position = transform.position;
            transform.DOScale(1.3f * Vector3.one, 0.25f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutQuint).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        }
    }
}
