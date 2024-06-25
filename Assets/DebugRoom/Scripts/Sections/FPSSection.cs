using ImUI;
using UnityEngine;

namespace DebugRoom
{
    public class FPSSection : DebugSection
    {
        public TextAdaptor fpsText;

        private LabelView fpsLabel;
        private bool overlay;
        private int targetFPS;

        protected override void OnUI()
        {
            ui.Label("FPS", new VPGetView((v) => fpsLabel = v as LabelView));

            overlay = ui.Toggle("Overlay", overlay);

            targetFPS = ui.Slider("Target FPS", targetFPS, 10, 120);

            ui.Row(() =>
            {
                if (ui.Button("15")) targetFPS = 15;
                if (ui.Button("30")) targetFPS = 30;
                if (ui.Button("60")) targetFPS = 60;
                if (ui.Button("120")) targetFPS = 120;
            });
        }

        private void Start()
        {
            targetFPS = 60;
        }

        private void Update()
        {
            var fps = 1f / Time.unscaledDeltaTime;
            var fpsString = "FPS: " + Mathf.FloorToInt(fps);

            if (fpsLabel != null)
            {
                fpsLabel.text.text = fpsString;
            }

            Application.targetFrameRate = targetFPS;

            fpsText.gameObject.SetActive(overlay);
            fpsText.text = fpsString;
        }
    }
}