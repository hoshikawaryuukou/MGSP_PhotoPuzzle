using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Views
{
    public sealed class GameOptionSelectorItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Graphic graphic;
        [SerializeField] private Color onColor = Color.white;
        [SerializeField] private Color offColor = Color.black;

        public Observable<Unit> Clicked => button.OnClickAsObservable();
        public bool IsSelected { get; private set; } = false;

        void Reset()
        {
            button = GetComponentInChildren<Button>();
            graphic = GetComponentInChildren<Graphic>();
            if (graphic != null)
            {
                offColor = graphic.color;
            }
        }

        public void SetSelected(bool selected)
        {
            graphic.color = selected ? onColor : offColor;
        }
    }
}
