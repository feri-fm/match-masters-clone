using System;
using System.Collections.Generic;
using ImUI;
using Match3;

namespace DebugRoom
{
    public class CommandsSection : DebugSection
    {
        public Dictionary<string, CommandInfo> commands = new();

        protected override void OnUI()
        {
            var game = manager.GetSection<GameSection>().game;
            foreach (var pair in commands)
            {
                ui.Tab(pair.Key, () =>
                {
                    pair.Value.view.Invoke(pair.Value.command, ui);
                    ui.disabled = game.isEvaluating;
                    if (ui.Button("Run"))
                    {
                        _ = game.RunCommand(pair.Value.command);
                    }
                    ui.disabled = false;
                    if (ui.Button("Reset"))
                    {
                        pair.Value.Reset();
                    }
                });
            }
        }

        private void Start()
        {
            commands.Clear();
            commands.Add("Shuffle", new CommandInfo<ShuffleCommand>((cmd, ui) =>
            {
                cmd.searchCount = ui.Slider("Search", cmd.searchCount, 20, 60);
                cmd.targetCount = ui.Slider("Target", cmd.targetCount, 0, 40);
            }));
            commands.Add("Rocket", new CommandInfo<RocketCommand>((cmd, ui) =>
            {
                cmd.searchCount = ui.Slider("Search", cmd.searchCount, 5, 30);
                cmd.targetCount = ui.Slider("Target", cmd.targetCount, 0, 20);
                cmd.timeDelay = ui.Slider("Delay", cmd.timeDelay, 0f, 0.4f);
            }));
            commands.Add("Duck", new CommandInfo<DuckCommand>((cmd, ui) =>
            {
                cmd.height = ui.Slider("Height", cmd.height, 0, manager.GetSection<GameSection>().game.height - 1);
                cmd.timeDelay = ui.Slider("Delay", cmd.timeDelay, 0f, 0.4f);
            }));
            commands.Add("Bucket", new CommandInfo<BucketCommand>((cmd, ui) =>
            {
                cmd.searchCount = ui.Slider("Search", cmd.searchCount, 10, 40);
                cmd.targetCount = ui.Slider("Target", cmd.targetCount, 0, 30);
                cmd.timeDelay = ui.Slider("Delay", cmd.timeDelay, 0f, 0.4f);
            }));
            commands.Add("Hat", new CommandInfo<HatCommand>((cmd, ui) =>
            {
                cmd.searchCount = ui.Slider("Search", cmd.searchCount, 10, 40);
                cmd.targetCount = ui.Slider("Target", cmd.targetCount, 0, 30);
                cmd.timeDelay = ui.Slider("Delay", cmd.timeDelay, 0f, 0.4f);
            }));
            commands.Add("TwoColors", new CommandInfo<TwoColorsCommand>((cmd, ui) =>
            {
            }));
        }

        public abstract class CommandInfo
        {
            public GameCommand command;
            public Action<GameCommand, ImUIBuilder> view;

            public CommandInfo(GameCommand command, Action<GameCommand, ImUIBuilder> view)
            {
                this.command = command;
                this.view = view;
            }

            public virtual void Reset()
            {

            }
        }
        public class CommandInfo<TCommand> : CommandInfo
            where TCommand : GameCommand, new()
        {
            public CommandInfo(Action<TCommand, ImUIBuilder> view) : base(new TCommand(), (cmd, ui) => view(cmd as TCommand, ui))
            {
            }

            public override void Reset()
            {
                base.Reset();
                command = new TCommand();
            }
        }
    }
}