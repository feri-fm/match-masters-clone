using System.Collections.Generic;
using MMC.Match3;
using UnityEngine;

namespace MMC.Game
{
    public class PipelineBot : Bot
    {
        public bool _build;
        public PipelineBotMiddleware[] middlewares;
        public MatchPatternGroup fiveMatch;
        public MatchPatternGroup fourMatch;
        public MatchPatternGroup threeMatch;
        public MatchPatternGroup twoMatch;

        protected override BotAction Think()
        {
            for (int i = 0; i < middlewares.Length; i++)
            {
                var middleware = middlewares[i];
                if (Random.value <= middleware.chance)
                {
                    var action = middleware.GetAction(this, gameplay);
                    if (action != null)
                        return action;
                }
            }

            for (int i = 0; i < middlewares.Length; i++)
            {
                var middleware = middlewares[i];
                var action = middleware.GetAction(this, gameplay);
                if (action != null)
                    return action;
            }

            return null;
        }

        private void OnValidate()
        {
            if (_build)
            {
                _build = false;
                // var items = new List<PipelineBotMiddleware>();
                // for (int i = 0; i < transform.childCount; i++)
                // {
                //     var child = transform.GetChild(i);
                //     if (child.gameObject.activeSelf)
                //     {
                //         var comp = child.GetComponent<PipelineBotMiddleware>();
                //         if (comp != null)
                //             items.Add(comp);
                //     }
                // }
                // middlewares = items.ToArray();
                middlewares = GetComponentsInChildren<PipelineBotMiddleware>();
            }
        }
    }
}