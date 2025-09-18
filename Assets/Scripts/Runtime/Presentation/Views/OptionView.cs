using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class OptionView : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private OptionButtonView[] widthNumberButton;
        [SerializeField] private OptionButtonView[] heightNumberButton;

        private int widthIndex = -1;
        private int heightIndex = -1;

        public Observable<Unit> PlayRequested => playButton.OnClickAsObservable();
        public Observable<Unit> CloseRequested => closeButton.OnClickAsObservable();
        public int SelectedWidthIndex => widthIndex;
        public int SelectedHeightIndex => heightIndex;

        public void SetWidthInfo(IList<string> labels, int widthIndex)
        {
            this.widthIndex = widthIndex;
            for (int i = 0; i < widthNumberButton.Length; i++)
            {
                var index = i;  
                widthNumberButton[i].SetLabel(labels[i]);
                widthNumberButton[i].SetSelected(i == widthIndex);
                widthNumberButton[i].Clicked.Subscribe(_ => widthIndex = index).AddTo(this);
            }
        }

        public void SetHeightInfo(IList<string> labels, int heightIndex)
        {
            this.heightIndex = heightIndex;
            for (int i = 0; i < heightNumberButton.Length; i++)
            {
                var index = i;
                heightNumberButton[i].SetLabel(labels[i]);
                heightNumberButton[i].SetSelected(i == heightIndex);
                heightNumberButton[i].Clicked.Subscribe(_ => heightIndex = index).AddTo(this);
            }
        }
    }
}
