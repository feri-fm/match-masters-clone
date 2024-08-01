using UnityEngine;
using MMC.EngineCore;
using System.Collections.Generic;
using System.Linq;

namespace MMC.Match3
{
    [CreateAssetMenu(fileName = "MatchConfig", menuName = "Match3/MatchConfig")]
    public class MatchConfig : ScriptableObject
    {
        public MatchPatternGroup search;
        public MatchPatternGroup moves;
        public MatchPatternGroup patterns;
    }

    [System.Serializable]
    public class MatchPatternGroup
    {
        public MatchPatternRotator[] patterns;

        private MatchPattern[] build;

        public MatchPattern[] BuildPatterns()
        {
            var res = new List<MatchPattern>();
            foreach (var item in patterns)
                res.AddRange(item.GetPatterns());
            build = res.ToArray();
            return build;
        }
        public MatchPattern[] GetPatterns()
        {
            if (build != null && build.Length > 0) return build;
            return BuildPatterns();
        }
    }

    [System.Serializable]
    public class MatchPatternRotator
    {
        public MatchPattern pattern;
        public MatchRotation rotation;

        public MatchPattern[] GetPatterns()
        {
            var res = new MatchPattern[rotation.flip ? rotation.rotations.Count * 2 : rotation.rotations.Count];
            for (int i = 0; i < rotation.rotations.Count; i++)
                res[i] = pattern.GetRotatedPattern(rotation.rotations[i]);
            if (rotation.flip)
            {
                var flipped = pattern.GetFlippedPattern();
                for (int i = 0; i < rotation.rotations.Count; i++)
                    res[rotation.rotations.Count + i] = flipped.GetRotatedPattern(rotation.rotations[i]);
            }
            return res;
        }
    }

    [System.Serializable]
    public class MatchRotation
    {
        // 0: 0, 1: 90, 2: 180, 3:170
        public List<int> rotations = new();
        public bool flip;
    }

    [System.Serializable]
    public class MatchPattern
    {
        public Reward reward;
        public int width = 5;
        public int height = 5;
        public Int2[] rewardPoints = new Int2[] { };
        public Int2[] points = new Int2[] { };

        public bool hasReward => rewardPoints.Length > 0;

        public enum Reward
        {
            None, Horizontal, Vertical, Bomb, Lightning
        }

        public MatchPattern DuplicatePattern()
        {
            return new MatchPattern
            {
                reward = reward,
                width = width,
                height = height,
                rewardPoints = rewardPoints.ToArray(),
                points = points.ToArray()
            };
        }

        public MatchPattern GetFlippedPattern()
        {
            var pattern = DuplicatePattern();

            for (int i = 0; i < pattern.points.Length; i++)
                pattern.points[i] = Flip(pattern.points[i]);
            for (int i = 0; i < pattern.rewardPoints.Length; i++)
                pattern.rewardPoints[i] = Flip(pattern.rewardPoints[i]);

            return pattern;

            Int2 Flip(Int2 v)
            {
                return new Int2(width - 1 - v.x, v.y);
            }
        }
        public MatchPattern GetRotatedPattern(int rotation)
        {
            var pattern = DuplicatePattern();
            if (rotation == 1 || rotation == 3)
            {
                pattern.width = height;
                pattern.height = width;
            }
            for (int i = 0; i < pattern.points.Length; i++)
                pattern.points[i] = Rotate(pattern.points[i], rotation);
            for (int i = 0; i < pattern.rewardPoints.Length; i++)
                pattern.rewardPoints[i] = Rotate(pattern.rewardPoints[i], rotation);

            var offset = (rotation % 4) switch
            {
                0 => Int2.zero,
                1 => new Int2(0, width - 1),
                2 => new Int2(width - 1, height - 1),
                3 => new Int2(height - 1, 0),
                _ => Int2.zero,
            };

            for (int i = 0; i < pattern.points.Length; i++)
                pattern.points[i] += offset;
            for (int i = 0; i < pattern.rewardPoints.Length; i++)
                pattern.rewardPoints[i] += offset;

            return pattern;

            Int2 Rotate(Int2 v, int turns = 1)
            {
                var res = v;
                for (int i = 0; i < turns; i++)
                    res = new Int2(res.y, -res.x);
                return res;
            }
        }
    }
}
