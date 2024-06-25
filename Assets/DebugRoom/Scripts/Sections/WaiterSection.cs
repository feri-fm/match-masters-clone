using System;
using System.Collections;
using System.Threading.Tasks;
using ImUI;
using UnityEngine;

namespace DebugRoom
{
    public class WaiterSection : DebugSection
    {
        private WaiterMode mode;

        private bool waiting;
        private float timeScale = 1;

        protected override void OnUI()
        {
            if (ui.Button($"Mode: {mode}"))
            {
                var modeInt = (int)mode;
                modeInt += 1;
                if (modeInt >= 3) modeInt = 0;
                mode = (WaiterMode)modeInt;

                waiting = false;
            }
            if (mode == WaiterMode.Manual)
            {
                ui.disabled = !waiting;
                if (ui.Button("Step Forward"))
                {
                    StepForward();
                }
                ui.disabled = false;
            }
            else if (mode == WaiterMode.Timer)
            {
                timeScale = ui.Slider("Time Scale", timeScale, 0.1f, 4f);
                ui.Row(() =>
                {
                    if (ui.Button("0.2")) timeScale = 0.2f;
                    if (ui.Button("0.5")) timeScale = 0.5f;
                    if (ui.Button("0.8")) timeScale = 0.8f;
                    if (ui.Button("1")) timeScale = 1;
                    if (ui.Button("1.5")) timeScale = 1.5f;
                    if (ui.Button("2")) timeScale = 2;
                });
            }
        }

        private int instantSteps = 0;
        public Task Wait(float time)
        {
            if (mode == WaiterMode.Instant)
            {
                instantSteps++;
                if (instantSteps % 100 != 0)
                    return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<byte>();
            StartCoroutine(IWait(time, () =>
            {
                tcs.SetResult(0);
            }));

            return tcs.Task;

            IEnumerator IWait(float time, Action callback)
            {
                if (mode == WaiterMode.Instant)
                {
                    yield return new WaitForEndOfFrame();
                }
                else if (mode == WaiterMode.Timer)
                {
                    yield return new WaitForSeconds(time / timeScale);
                }
                else
                {
                    waiting = true;
                    imUI.Changed();
                    while (waiting)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                callback.Invoke();
            }
        }

        public void StepForward()
        {
            if (waiting)
            {
                waiting = false;
            }
        }

        public enum WaiterMode
        {
            Timer = 0,
            Manual = 1,
            Instant = 2
        }
    }
}