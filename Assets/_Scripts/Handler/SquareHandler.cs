using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Handler
{
    public class SquareHandler : MonoBehaviour, IPointerClickHandler
    {
        //Position
        public int col { get; private set; }

        public int row { get; private set; }

        public bool isScore;
        public bool hasBall => ball != null && ball.gameObject.activeSelf;
        public BallHandler ball;

        public void OnPointerClick(PointerEventData eventData)
        {
            GameManager.Instance.TargetSelect(this);
        }

        public void Initialize(int x, int y)
        {
            ball = null;
            col = x;
            row = y;
            isScore = false;
        }

        public void Score()
        {
            ball.gameObject.SetActive(false);
            isScore = false;
            ball = null;
        }
    }
}
