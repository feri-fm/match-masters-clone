using UnityEngine;
using UnityEngine.UI;

namespace ImUI
{
    public class ImageView : View<ImageViewState>
    {
        public Image image;
        public RawImage rawImage;

        protected override void LoadState(ImageViewState state)
        {
            base.LoadState(state);
            image.sprite = state.sprite;
            rawImage.texture = state.texture;

            image.enabled = state.sprite != null;
            rawImage.enabled = state.texture != null;
        }
    }

    public class ImageViewState : ViewState
    {
        public Sprite sprite;
        public Texture texture;

        public ImageViewState(Sprite sprite)
        {
            this.sprite = sprite;
        }
        public ImageViewState(Texture texture)
        {
            this.texture = texture;
        }
    }
}