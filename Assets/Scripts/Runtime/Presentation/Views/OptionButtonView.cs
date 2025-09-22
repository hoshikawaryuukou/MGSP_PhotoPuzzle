using R3;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniGears.UGUIKit.Controls;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class OptionButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;
        [SerializeField] private ButtonColorSwitcher buttonColorSwitcher;

        public Observable<Unit> Clicked => button.OnClickAsObservable();

        public void SetSelected(bool selected) 
        {
            buttonColorSwitcher.SetSelected(selected);
        }

        public void SetLabel(string text) 
        {
            label.text = text;
        }
    }
}
