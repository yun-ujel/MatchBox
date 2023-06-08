using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MatchBox.UI.Pause
{
    public class PauseMenu : MonoBehaviour
    {
        #region Properties

        #region Static

        public static PauseMenu Instance { get; private set; }

        public static bool Paused { get; private set; }

        public static event System.EventHandler OnPaused;
        public static event System.EventHandler OnResume;

        #endregion

        #region Menus
        [System.Serializable]
        public class Menu
        {
            public int previousMenuIndex;

            public GameObject menuParent;
            public Selectable firstSelected;
        }

        public Menu[] menus;
        private int currentSelectedMenu;

        #endregion

        #endregion

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }

            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            CloseAllMenus();
        }

        #region Input Methods
        public void ReceivePauseInput(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) { return; }

            if (!Paused)
            {
                OpenMenu(0);
                return;
            }

            GoToPreviousMenu();
        }
        #endregion

        #region Pause Methods
        public void Pause()
        {
            Time.timeScale = 0f;

            Paused = true;
            OnPaused?.Invoke(this, System.EventArgs.Empty);
        }

        public void Resume()
        {
            Time.timeScale = 1f;

            Paused = false;
            OnResume?.Invoke(this, System.EventArgs.Empty);
        }
        #endregion

        #region Menu Methods
        public void OpenMenu(int index)
        {
            currentSelectedMenu = index;

            if (!Paused)
            {
                Pause();
            }

            for (int i = 0; i < menus.Length; i++)
            {
                if (i == index)
                {
                    menus[i].menuParent.SetActive(true);
                    if (menus[i].firstSelected != null) { menus[i].firstSelected.Select(); }
                    continue;
                }

                menus[i].menuParent.SetActive(false);
            }
        }

        public void GoToPreviousMenu()
        {
            if (currentSelectedMenu < 1 || currentSelectedMenu > menus.Length)
            {
                CloseAllMenus();
                return;
            }

            Menu selectedMenu = menus[currentSelectedMenu];
            if (selectedMenu == null || selectedMenu.previousMenuIndex < 0 || selectedMenu.previousMenuIndex > menus.Length)
            {
                CloseAllMenus();
                return;
            }

            OpenMenu(selectedMenu.previousMenuIndex);
        }

        private void CloseAllMenus()
        {
            currentSelectedMenu = -1;
            for (int i = 0; i < menus.Length; i++)
            {
                menus[i].menuParent.SetActive(false);
            }

            if (Paused)
            {
                Resume();
            }
        }
        #endregion
    }

}