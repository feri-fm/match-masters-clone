using MMC.Core;
using MMC.Match3;
using UnityEngine;
using UnityEngine.UI;

namespace MMC.DebugRoom
{
    public class UndoRedoSection : DebugSection
    {
        public UndoRedoManager undoRedoManager;

        public Member<Selectable> undo;
        public Member<Selectable> redo;

        private GameSection gameSection;
        private SaveSection saveSection;

        protected override void OnUI()
        {
            UIOverlayToggle();
            ui.Row(() =>
            {
                ui.disabled = gameSection.game.isEvaluating || !undoRedoManager.canUndo;
                if (ui.Button("< " + (undoRedoManager.undoAction?.undoTitle ?? "Undo")))
                {
                    Undo();
                }
                ui.disabled = gameSection.game.isEvaluating || !undoRedoManager.canRedo;
                if (ui.Button((undoRedoManager.redoAction?.redoTitle ?? "Redo") + " >"))
                {
                    Redo();
                }
                ui.disabled = false;
            });
        }

        protected override void Setup()
        {
            base.Setup();
            gameSection = manager.GetSection<GameSection>();
            saveSection = manager.GetSection<SaveSection>();

            gameSection.onEngineCreated += (engine) =>
            {
                gameSection.game.onTrySwap += (tileA, tileB) =>
                {
                    var undoData = saveSection.Save();
                    var redoA = tileA.position;
                    var redoB = tileB.position;

                    undoRedoManager.AddAction(
                    "Swap", () =>
                    {
                        var gameOptions = gameSection.gameOptions.JsonCopy();
                        saveSection.Load(undoData);
                        gameSection.gameOptions = gameOptions;
                    },
                    "Swap", () =>
                    {
                        gameSection.game.TrySwap(redoA, redoB, true);
                    });
                };
            };
        }

        private void LateUpdate()
        {
            undo.value.interactable = !gameSection.game.isEvaluating && undoRedoManager.canUndo;
            redo.value.interactable = !gameSection.game.isEvaluating && undoRedoManager.canRedo;
        }

        [Member]
        public void Undo()
        {
            if (!gameSection.game.isEvaluating)
            {
                undoRedoManager.Undo();
                imUI.Changed();
            }
        }

        [Member]
        public void Redo()
        {
            if (!gameSection.game.isEvaluating)
            {
                undoRedoManager.Redo();
                imUI.Changed();
            }
        }
    }
}