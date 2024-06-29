using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMC.EngineCore
{
    public class EngineScheduler : MonoBehaviour
    {
        public EngineView engineView;
        public List<EngineSchedulerTask> tasks = new List<EngineSchedulerTask>();

        public float time;

        public bool isPlaying;

        private void Update()
        {
            if (isPlaying)
            {
                time += Time.deltaTime;
                for (int i = tasks.Count - 1; i >= 0; i--)
                {
                    var task = tasks[i];
                    if (time >= task.time)
                    {
                        task.action.Invoke(engineView);
                        tasks.RemoveAt(i);
                    }
                }
            }
        }

        public void Stop()
        {
            isPlaying = false;
            time = 0;
            tasks.Clear();
        }
        public void Play()
        {
            isPlaying = true;
        }
        public void Pause()
        {
            isPlaying = false;
        }

        public EngineSchedulerTask ScheduleTask(float time, Action<EngineView> action)
        {
            var task = new EngineSchedulerTask(this, time, action);
            tasks.Add(task);
            return task;
        }
    }

    public class EngineSchedulerTask
    {
        public float time;
        public Action<EngineView> action;
        public EngineScheduler scheduler;

        public EngineSchedulerTask(EngineScheduler scheduler, float time, Action<EngineView> action)
        {
            this.scheduler = scheduler;
            this.time = time;
            this.action = action;
        }
    }
}