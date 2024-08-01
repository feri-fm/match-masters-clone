using System.Collections.Generic;
using System.Linq;
using MMC.Match3;
using UnityEngine;

namespace MMC.Game
{
    public class MatchBotMiddleware : PipelineBotMiddleware
    {
        public ScanDirection direction;
        public TileColor color;
        public BotMatchGroup matchGroup;
        public List<BotTileFilter> priorities;
        public List<BotTileFilter> includeAll;
        public List<BotTileFilter> includeAny;
        public List<BotTileFilter> exclude;

        public TileColor mappedColor => (bot?.isOpponent ?? false) ? (color == TileColor.blue ? TileColor.red : (color == TileColor.red ? TileColor.blue : color)) : color;

        protected override BotAction GetAction()
        {
            if (matchGroup == BotMatchGroup.TwoPowerUps)
            {
                var swaps = new List<SwapAction>();
                game.ScanPattern(bot.twoMatch.GetPatterns(), direction, (offset, pattern) =>
                {
                    var a = offset + pattern.points[0];
                    var b = offset + pattern.points[1];
                    var tileA = game.GetTileAt(a);
                    var tileB = game.GetTileAt(b);
                    if (tileA.HasTrait<PowerUpTrait>() && tileB.HasTrait<PowerUpTrait>())
                        swaps.Add(new SwapAction(tileA, tileB));
                    return false;
                });

                foreach (var priority in priorities)
                {
                    var swap = swaps.Find(e => CheckFilter(e.tileA, priority) || CheckFilter(e.tileB, priority));
                    if (swap != null)
                        return swap;
                }
            }
            else
            {
                var patterns = matchGroup switch
                {
                    BotMatchGroup.Five => bot.fiveMatch.GetPatterns(),
                    BotMatchGroup.Four => bot.fourMatch.GetPatterns(),
                    BotMatchGroup.Three => bot.threeMatch.GetPatterns(),
                    _ => bot.threeMatch.GetPatterns(),
                };

                var matches = game.ScanAll(patterns, direction, mappedColor, CheckMatch);
                foreach (var priority in priorities)
                {
                    var match = matches.Find(e => e.tiles.Any(e => CheckFilter(e, priority)));
                    if (match != null)
                        return GetSwapAction(match);
                }

                if (priorities.Count == 0 && matches.Count > 0)
                    return GetSwapAction(matches.Random());
            }
            return null;

            bool CheckMatch(Match match)
            {
                foreach (var filter in exclude)
                    if (match.tiles.Any(e => CheckFilter(e, filter)))
                        return false;

                foreach (var filter in includeAll)
                    if (!match.tiles.Any(e => CheckFilter(e, filter)))
                        return false;

                if (includeAny.Count > 0)
                {
                    var found = false;
                    foreach (var filter in includeAny)
                        if (match.tiles.Any(e => CheckFilter(e, filter)))
                        {
                            found = true;
                            break;
                        }
                    if (!found) return false;
                }

                return true;
            }
        }

        public bool CheckFilter(Tile tile, BotTileFilter filter)
        {
            switch (filter)
            {
                case BotTileFilter.Any: return true;
                case BotTileFilter.Bead: if (tile is BeadTile) return true; break;
                case BotTileFilter.PowerUp: if (tile.HasTrait<PowerUpTrait>()) return true; break;
                case BotTileFilter.Mechanic: if (tile.HasTrait<MechanicTrait>()) return true; break;
                case BotTileFilter.Bomb: if (tile is BombTile) return true; break;
                case BotTileFilter.Lightning: if (tile is LightningTile) return true; break;
                case BotTileFilter.ClearLine: if (tile is ClearLineTile) return true; break;
                case BotTileFilter.ClearLineHorizontal: if (tile is ClearLineTile clear && clear.prefab.direction == ClearLineDirection.Horizontal) return true; break;
                case BotTileFilter.ClearLineVertical: if (tile is ClearLineTile clear1 && clear1.prefab.direction == ClearLineDirection.Vertical) return true; break;
            }
            return false;
        }

        public SwapAction GetSwapAction(Match match)
        {
            var offset = match.offset;
            var a = offset + match.pattern.rewardPoints[0];
            var b = offset + match.pattern.rewardPoints[1];
            return new SwapAction(a, b);
        }

        private void OnValidate()
        {
            var res = $"{Mathf.FloorToInt(chance * 100)}%";
            if (direction != ScanDirection.Default)
                res += $" {direction}";
            res += $" {matchGroup}";
            if (mappedColor != TileColor.none)
                res += $" {mappedColor}";

            var include = new List<BotTileFilter>();
            include.AddRange(includeAny);
            include.AddRange(includeAll);

            if (priorities.Count > 0)
            {
                res += " P:";
                for (int i = 0; i < priorities.Count; i++)
                {
                    res += $"{priorities[i]}";
                    if (i < priorities.Count - 1) res += ",";
                }
            }

            if (include.Count > 0)
            {
                res += " I:";
                for (int i = 0; i < include.Count; i++)
                {
                    res += $"{include[i]}";
                    if (i < include.Count - 1) res += ",";
                }
            }

            if (exclude.Count > 0)
            {
                res += " E:";
                for (int i = 0; i < exclude.Count; i++)
                {
                    res += $"{exclude[i]}";
                    if (i < exclude.Count - 1) res += ",";
                }
            }

            name = res;
        }
    }

    public enum BotMatchGroup
    {
        Three, Four, Five, TwoPowerUps
    }

    public enum BotTileFilter
    {
        Any,
        Bead,
        PowerUp,
        Mechanic,
        Bomb,
        Lightning,
        ClearLine,
        ClearLineHorizontal,
        ClearLineVertical,
    }
}