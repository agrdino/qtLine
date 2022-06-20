using System.Collections;
using System.Collections.Generic;
using _Prefab.Popup;
using _Scripts.Scene;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static qtScene;
using static qtHelper;

namespace _Scripts.System
{
    public class UIManager : qtSingleton<UIManager>
    {
        [Header("Scene config")] [SerializeField]
        private EScene startScene;

        public popBase currentPopup
        {
            get;
            private set;
        }

        public sceneBase currentScene
        {
            get;
            private set;
        }

        public hudBase currentHUD
        {
            get;
            private set;
        }


        #region ----- VARIABLE -----

        private GameObject _canvasOnTop;
        private GameObject canvas;
        private GameObject _ignoreCast;
        private Image _fading;

        private Dictionary<EPopup, popBase> _popups;
        private Dictionary<EScene, sceneBase> _scenes;
        private Dictionary<EHud, hudBase> _huds;
        public Stack<popBase> stackPopup;
        
        #endregion

        #region ----- INITIALIZE -----

        private void Start()
        {
            InitObject();
            Initialize();
            ShowScene(startScene);
        }

        private void InitObject()
        {
            _canvasOnTop = FindObjectInRootIncludingInactive("CanvasOnTop");
            canvas = FindObjectInRootIncludingInactive("Canvas");
        }
        
        private void Initialize()
        {
            stackPopup ??= new Stack<popBase>();
        }

        #endregion

        #region ----- UNITY EVENT -----

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (stackPopup.Count > 0)
                {
                    stackPopup.Pop().Hide();
                    if (stackPopup.Count > 0)
                    {
                        currentPopup = stackPopup.Peek();
                    }
                }
            }
        }

        #endregion

        #region ----- PUBLIC FUNCTION -----
        
        public bool ignoreCast
        {
            set {
                if (_ignoreCast == null)
                {
                    _ignoreCast = Instantiate(Resources.Load<GameObject>("imgIgnoreCast"), _canvasOnTop.transform);
                    _ignoreCast.transform.SetSiblingIndex(1);
                }

                _ignoreCast.SetActive(value);
            }
        }

        private Color _fade;
        public bool fading
        {
            set
            {
                _fade = Color.clear;
                _fade.a = 150 / 255f;
                if (_fading == null)
                {
                    _fading = Instantiate(Resources.Load<GameObject>("imgFading"), _canvasOnTop.transform).GetComponent<Image>();
                    _fading.gameObject.SetActive(false);
                    _fading.transform.SetSiblingIndex(1);
                    _fading.color = Color.clear;
                }
                if (value)
                {
                    if (_fading.gameObject.activeSelf)
                    {
                        return;
                    }
                    _fading.gameObject.SetActive(true);
                    _fading.DOColor(_fade, 0.25f);
                }
                else
                {
                    _fading.DOColor(Color.clear, 0.25f).OnComplete(() => _fading.gameObject.SetActive(false));
                }
            }
        }
        
        public popBase ShowPopup(EPopup popup)
        {
            _popups ??= new Dictionary<EPopup, popBase>();
            popBase tempPopup = null;
            if (!_popups.ContainsKey(popup))
            {
                tempPopup = Instantiate(sceneData.popups.Find(x => x.id == popup).prefab, _canvasOnTop.transform).GetComponent<popBase>();
                _popups.Add(popup, tempPopup);
            }
            else
            {
                tempPopup = _popups[popup];
            }
            tempPopup.transform.SetSiblingIndex(_canvasOnTop.transform.childCount - 1);
            currentPopup = tempPopup.Show();
            return tempPopup;
        }

        public popBase GetPopup(EPopup popup)
        {
            return _popups.ContainsKey(popup) ? _popups[popup] : null;
        }

        public sceneBase ShowScene(EScene scene)
        {
            _scenes ??= new Dictionary<EScene, sceneBase>();
            
            sceneBase tempScene = null;
            var showScene = sceneData.sences.Find(x => x.id == scene);

            if (!_scenes.ContainsKey(scene))
            {
                var temp = FindObjectInChildren(canvas, showScene.scene.name);
                if (temp == null)
                {
                    tempScene = Instantiate(sceneData.sences.Find(x => x.id == scene).scene, canvas.transform).GetComponent<sceneBase>();
                    tempScene.InitObject();
                    _scenes.Add(scene, tempScene);
                }
                else
                {
                    tempScene = temp.GetComponent<sceneBase>();
                    tempScene.InitObject();
                    _scenes.Add(scene, tempScene);
                }
            }
            else
            {
                tempScene = _scenes[scene];
            }

            tempScene.Initialize();

            if (currentScene != null)
            {
                currentScene.Hide();
                StartCoroutine(FadingScene(tempScene));
            }
            else
            {
                currentScene = tempScene;
                tempScene.Show();
            }

            currentHUD = ShowHUD(showScene);

            return tempScene;
        }

        public sceneBase GetScene(EScene scene)
        {
            return _scenes.ContainsKey(scene) ? _scenes[scene] : null;
        }
        
        #endregion

        #region ----- PRIVATE FUNCTION -----

        private hudBase ShowHUD(qtObject.Scene scene)
        {
            if (!scene.showHUD) 
            {
                if (currentHUD != null)
                {
                    currentHUD.Hide();
                }
                return null;
            }
            _huds ??= new Dictionary<EHud, hudBase>();

            hudBase tempHud = null;
            if (!_huds.ContainsKey(scene.hudId))
            {
                var hudName = sceneData.huds.Find(x => x.id == scene.hudId).prefab.name;
                var temp = FindObjectInChildren(_canvasOnTop, hudName);
                if (temp == null)
                {
                    tempHud = Instantiate(sceneData.huds.Find(x => x.id == scene.hudId).prefab, _canvasOnTop.transform).GetComponent<hudBase>();
                    tempHud.InitObject();
                    _huds.Add(scene.hudId, tempHud);
                }
                else
                {
                    tempHud = temp.GetComponent<hudBase>();
                    tempHud.InitObject();
                    _huds.Add(scene.hudId, tempHud);
                }
            }
            else
            {
                tempHud = _huds[scene.hudId];
            }
            tempHud.transform.SetSiblingIndex(0);
            tempHud.Initialize();
            tempHud.Show();
            return tempHud;
        }

        private IEnumerator FadingScene(sceneBase newScene)
        {
            Fading(true, Color.black);
            yield return new WaitForSeconds(0.5f);
            currentScene = newScene;
            newScene.Show();
            Fading(false, Color.black);
        }

        private void Fading(bool fade, Color color)
        {
            _fading.DOKill();
            _fade = color;
            _fade.a = 1;
            if (_fading == null)
            {
                _fading = Instantiate(Resources.Load<GameObject>("imgFading"), _canvasOnTop.transform).GetComponent<Image>();
                _fading.gameObject.SetActive(false);
                _fading.transform.SetSiblingIndex(1);
                _fading.color = Color.clear;
            }
            if (fade)
            {
                _fading.gameObject.SetActive(true);
                _fading.DOColor(_fade, 0.5f);
            }
            else
            {
                _fading.DOColor(Color.clear, 0.25f).SetDelay(0.25f).OnComplete(() => _fading.gameObject.SetActive(false));
            }
        }
        
        #endregion
    }
}
