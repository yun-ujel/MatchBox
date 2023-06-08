using UnityEngine.UI;
using UnityEngine;

namespace MatchBox.UI.Pause
{
    [RequireComponent(typeof(Graphic))]
    public class PauseBackground : MonoBehaviour
    {
        private Graphic graphic;

        private void Start()
        {
            graphic = GetComponent<Graphic>();

            PauseMenu.OnPaused += OnPaused;
            PauseMenu.OnResume += OnResume;

            if (!PauseMenu.Paused)
            {
                OnResume(this, System.EventArgs.Empty);
            }
        }

        private void OnResume(object sender, System.EventArgs e)
        {
            graphic.CrossFadeAlpha(0f, 0.1f, true);
        }

        private void OnPaused(object sender, System.EventArgs e)
        {
            graphic.CrossFadeAlpha(0.8f, 0.1f, true);
        }
    }

}