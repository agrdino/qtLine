using _Scripts.System;
using UnityEngine;

namespace _Scripts.Scene
{
    public abstract class sceneBase : MonoBehaviour
    {
        public virtual void Initialize()
        {
            InitEvent();
        }
        protected abstract void InitEvent();
        public abstract void InitObject();
        public abstract void RemoveEvent();

        public virtual void Show()
        {
            UIManager.Instance.ignoreCast = false;
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            RemoveEvent();
            gameObject.SetActive(false);
        }
    }
}
