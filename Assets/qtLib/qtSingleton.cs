using UnityEngine;

public abstract class qtSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance { 
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            var gameObject = new GameObject();
            gameObject.name = nameof(T);
            _instance = gameObject.AddComponent<T>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this as T;
        }
        Init();
    }
    
    protected virtual void Init(){}
}