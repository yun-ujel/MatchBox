using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MatchBox.UI.Pause
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool Paused { get; private set; }

        public static event System.EventHandler OnPaused;
        public static event System.EventHandler OnResume;

        [System.Serializable]
        public class Menu
        {
            public GameObject menuParent;
            public Selectable firstSelected;
        }

        public Menu[] menus;
        private int currentSelectedMenu;

        private PlayerInput playerInput;
        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            Resume();
        }

        #region Input Methods
        public void ReceivePauseInput(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) { return; }

            if (Paused)
            {
                Resume();
                return;
            }

            Pause();
        }
        #endregion

        #region Pause Methods
        public void Pause(bool openFirstMenu = true)
        {
            Time.timeScale = 0f;

            Paused = true;
            OnPaused?.Invoke(this, System.EventArgs.Empty);
            playerInput.SwitchCurrentActionMap("Menu");

            if (openFirstMenu)
            {
                OpenMenu(0);
            }
        }

        public void Resume()
        {
            Time.timeScale = 1f;

            Paused = false;
            OnResume?.Invoke(this, System.EventArgs.Empty);
            playerInput.SwitchCurrentActionMap("Box");

            CloseMenus();
        }
        #endregion

        #region Menu Methods
        public void OpenMenu(int index)
        {
            if (index < 0)
            {
                Resume();
            }

            currentSelectedMenu = index;

            if (!Paused)
            {
                Pause(false);
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

        private void CloseMenus()
        {
            currentSelectedMenu = -1;
            for (int i = 0; i < menus.Length; i++)
            {
                menus[i].menuParent.SetActive(false);
            }
        }
        #endregion
    }

}