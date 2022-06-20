using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Prefab.Popup.NotiPopup
{
    public class NotiPopup : popBase
    {
        [SerializeField] private Button _btnOk;
        private Action _evtConfirm;
        public NotiPopup Initialize(string content, Action action = null)
        {
            _evtConfirm = action;
            _txtContent.text = content;
            return this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _btnOk.onClick.AddListener(OnButtonConfirmClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _evtConfirm = null;
            _btnOk.onClick.RemoveAllListeners();
        }

        private void OnButtonConfirmClick()
        {
            _evtConfirm?.Invoke();
            Hide();
        }
    }
}