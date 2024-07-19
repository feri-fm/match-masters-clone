using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BuildSystem
{
    public class BuildScript
    {
        [MenuItem("Build System/Set Windows Local")]
        public static void SetWindows()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            Builder.Begin().SetServiceGroupName("local");
        }

        [MenuItem("Build System/Windows Local")]
        public static void WindowsLocal()
        {
            Builder.Begin()
                .SetServiceGroupName("local")
                .SetWindows()
                .SetPath($"{GetTime()}-windows-local", "match-masters.exe")
                .Build();
        }

        [MenuItem("Build System/Linux Server Stage")]
        public static void LinuxServerStage()
        {
            Builder.Begin()
                .SetServiceGroupName("stage")
                .SetLinuxServer()
                .SetPath($"{GetTime()}-linux-server-stage", "server.x86_64")
                .Build();
        }

        [MenuItem("Build System/Android Stage")]
        public static void AndroidStage()
        {
            Builder.Begin()
                .SetServiceGroupName("stage")
                .SetAndroid()
                .SetPath($"{GetTime()}-android-stage", "match-masters-stage.apk")
                .Build();
        }

        [MenuItem("Build System/Linux Server and Android Stage")]
        public static void Stage()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                AndroidStage();
                LinuxServerStage();
            }
            else
            {
                LinuxServerStage();
                AndroidStage();
            }
        }

        public static string GetTime()
        {
            return DateTime.Now.ToString("HH-mm-ss");
        }
    }

    public class Builder
    {
        public BuildPlayerOptions buildPlayerOptions;
        public ServiceManagerConfig serviceManagerConfig;

        private Builder()
        {
            buildPlayerOptions = GetBuildPlayerOptions();
            var guids = AssetDatabase.FindAssets("t:ServiceManagerConfig");
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            serviceManagerConfig = AssetDatabase.LoadAssetAtPath<ServiceManagerConfig>(path);

        }

        public static Builder Begin()
        {
            var builder = new Builder();
            return builder;
        }

        public Builder SetServiceGroupName(string name)
        {
            serviceManagerConfig.SetGroupName(name);
            return this;
        }

        public Builder SetWindows()
        {
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Player;
            return this;
        }
        public Builder SetLinuxServer()
        {
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
            buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
            buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Server;
            return this;
        }
        public Builder SetAndroid()
        {
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Player;
            return this;
        }

        public Builder SetPath(string path)
        {
            buildPlayerOptions.locationPathName = GetPath(path);
            return this;
        }
        public Builder SetPath(string folder, string name)
        {
            buildPlayerOptions.locationPathName = GetPath(folder, name);
            return this;
        }

        public Builder Build()
        {
            var time = DateTime.Now;
            try
            {
                BuildPipeline.BuildPlayer(buildPlayerOptions);
                var delta = DateTime.Now - time;
                Debug.Log($"Build completed in {delta}");
            }
            catch (Exception e)
            {
                var delta = DateTime.Now - time;
                Debug.LogError(e.Message + "\n" + e.StackTrace);
                Debug.LogError($"Build failed in {delta}");
                throw e;
            }
            return this;
        }

        public string GetPath(string path)
        {
            var time = DateTime.Now;
            var prj = new DirectoryInfo(Application.dataPath).Parent.Parent.FullName;
            var dir = Path.Combine(prj, "Builds", $"{time:yyyy-MM-dd}");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, path);
        }
        public string GetPath(string folder, string name)
        {
            var dir = GetPath(folder);
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, name);
        }

        public BuildPlayerOptions GetBuildPlayerOptions()
        {
            return new BuildPlayerOptions()
            {
                scenes = EditorBuildSettings.scenes.Select(e => e.path).ToArray(),
                locationPathName = "C:/FeriFm/Projects/Match Masters Clone/Build/test-build.apk",
            };
        }
    }
}