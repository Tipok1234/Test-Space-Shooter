using System.Collections.Generic;
using UnityEngine;
using Screens;

namespace Managers
{
    [DefaultExecutionOrder(10)]
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<BaseScreen> screensPrefab = new List<BaseScreen>();
        [SerializeField] private List<BaseScreen> screens = new List<BaseScreen>();

        [SerializeField] private Camera camera;

        public void OpenScreen<T>() where T : BaseScreen
        {
            T screen = GetScreen<T>();

            if (screen != null)
            {
                screen.OpenScreen();
            }
            else
            {
                Debug.LogError($"Screen of type {typeof(T)} is not registered.");
            }
        }

        public void CloseScreen<T>() where T : BaseScreen
        {
            T screen = GetScreen<T>();

            if (screen != null)
            {
                screen.CloseScreen();
            }
            else
            {
                Debug.LogError($"Screen of type {typeof(T)} is not registered.");
            }
        }

        public T GetScreen<T>() where T : BaseScreen
        {
            foreach (var screen in screens)
            {
                if (screen is T)
                {
                    return (T)screen;
                }
            }

            return null;
        }
        
        public void Init()
        {
            for (int i = 0; i < screensPrefab.Count; i++)
            {
                var addScreen = Instantiate(screensPrefab[i], gameObject.transform);
                addScreen.SetCamera(camera);
                addScreen.CloseScreen();
                screens.Add(addScreen);
            }
        }
    }
}
