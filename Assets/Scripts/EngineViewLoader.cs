using System.Collections;
using System.Collections.Generic;
using Core;
using Match3;
using UnityEngine;

public class EngineViewLoader : MonoBehaviour
{
    public EngineView engineView;
    public EngineConfig engineConfig;
    public GameConfig gameConfig;

    private Engine engine;

    public GameEntity game;

    private void Start()
    {
        engine = new Engine(engineConfig);
        engineView.Setup(engine);

        game = engine.CreateEntity("game") as GameEntity;
        game.Setup(gameConfig, new GameOptions());

        engine.Evaluate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            engine.Evaluate();
    }
}
