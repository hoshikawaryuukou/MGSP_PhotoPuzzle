using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Game.Views
{
    public sealed class GamePlayMenuSheet : MonoBehaviour
    {
        [SerializeField] private Button replayButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button clearButton;

        public Observable<Unit> ReplayRequested => replayButton.OnClickAsObservable();
        public Observable<Unit> NewGameRequested => newGameButton.OnClickAsObservable();
        public Observable<Unit> ClearConfirmed => clearButton.OnClickAsObservable();

        public void SetInteractable(bool value)
        {
            replayButton.interactable = value;
            newGameButton.interactable = value;
            clearButton.interactable = value;
        }

        public void SetPlaying()
        {
            replayButton.gameObject.SetActive(true);
            newGameButton.gameObject.SetActive(true);
            clearButton.gameObject.SetActive(false);
        }

        public void SetIdle()
        {
            replayButton.gameObject.SetActive(true);
            newGameButton.gameObject.SetActive(true);
            clearButton.gameObject.SetActive(false);
        }

        public void SetCompleted()
        {
            replayButton.gameObject.SetActive(false);
            newGameButton.gameObject.SetActive(false);
            clearButton.gameObject.SetActive(true);
        }
    }
}
