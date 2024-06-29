using UnityEditor;

namespace BuildSystem
{
    public class BuildScript
    {
        [MenuItem("BuildSystem/PerformBuild")]
        public static void PerformBuild()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = new string[] { "Assets/Core/DebugRoom/DebugRoom.unity" },
                locationPathName = "C:/FeriFm/Projects/Match Masters Clone/Build/test-build.apk",
                targetGroup = BuildTargetGroup.Android,
                target = BuildTarget.Android,
            };
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}