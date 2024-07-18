using System;
using System.Collections.Generic;
using ImUI;
using MMC.Game;
using MMC.Match3;

namespace MMC.DebugRoom
{
    public class CommandsSection : DebugSection
    {
        public Dictionary<string, CommandInfo> commands = new();

        private GameSection gameSection;
        private UndoRedoSection undoRedoSection;
        private SaveSection saveSection;

        protected override void Setup()
        {
            base.Setup();
            gameSection = manager.GetSection<GameSection>();
            undoRedoSection = manager.GetSection<UndoRedoSection>();
            saveSection = manager.GetSection<SaveSection>();
        }

        protected override void OnUI()
        {
            foreach (var pair in commands)
            {
                ui.Tab(pair.Key, () =>
                {
                    pair.Value.view.Invoke(pair.Value.command, ui);
                    ui.Row(() =>
                    {
                        if (ui.Button("Reset"))
                        {
                            pair.Value.Reset();
                        }
                        ui.disabled = gameSection.game.isEvaluating;
                        if (ui.Button("Run"))
                        {

                            var undoData = saveSection.Save();

                            var commandData = new JsonData();
                            JsonData.Save(pair.Value.command, commandData);

                            var commandCopy = pair.Value.CreateCommand();
                            JsonData.Load(commandCopy, commandData);

                            undoRedoSection.undoRedoManager.AddAction(
                            pair.Key, () =>
                            {
                                var gameOptions = gameSection.gameOptions.JsonCopy();
                                saveSection.Load(undoData);
                                gameSection.gameOptions = gameOptions;
                            },
                            pair.Key, () =>
                            {
                                _ = gameSection.game.RunCommand(commandCopy);
                            });

                            _ = gameSection.game.RunCommand(pair.Value.command);
                        }
                        ui.disabled = false;
                    });
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
            commands.Add("Rocket", new CommandInfo<RocketBoxCommand>((cmd, ui) =>
            {
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
                ui.Row(() =>
                {
                    var color = cmd.colorValue;
                    ui.disabled = color <= 0;
                    if (ui.Button("<", new VPLayoutMinWidth(80)))
                    {
                        color--;
                    }
                    ui.disabled = false;
                    ui.Label(new TileColor(cmd.colorValue).ToString(), new VPLayoutFlexibleWidth(100));
                    ui.disabled = color >= 5;
                    if (ui.Button(">", new VPLayoutMinWidth(80)))
                    {
                        color++;
                    }
                    ui.disabled = false;

                    cmd.colorValue = color;
                });
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

            public virtual GameCommand CreateCommand()
            {
                return null;
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

            public override GameCommand CreateCommand()
            {
                base.CreateCommand();
                var command = new TCommand();
                return command;
            }

            public override void Reset()
            {
                base.Reset();
                command = new TCommand();
            }
        }
    }
}