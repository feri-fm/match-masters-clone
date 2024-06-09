using System.Collections;
using System.Collections.Generic;
using Match3;
using UnityEngine;

public class EngineViewLoader : MonoBehaviour
{
    public EngineView engineView;
    public EngineConfig engineConfig;

    private Engine engine;

    public GameEntity game;

    private void Start()
    {
        engine = new Engine(engineConfig, new EngineOptions());
        engineView.Setup(engine);

        game = engine.CreateEntity("game") as GameEntity;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            engine.Evaluate();
    }
}
