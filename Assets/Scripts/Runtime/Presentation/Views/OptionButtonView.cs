using R3;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class OptionButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color normalColor;
        [SerializeField] private TMP_Text label;

        public Observable<Unit> Clicked => button.OnClickAsObservable();

        public void SetSelected(bool selected) 
        {
            backgroundImage.color = selected ? selectedColor : normalColor;
        }

        public void SetLabel(string text) 
        {
            label.text = text;
        }
    }
}
