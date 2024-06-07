using System.Collections;
using System.Collections.Generic;
using Match3;
using UnityEngine;

public class EngineViewLoader : MonoBehaviour
{
    public EngineView engineView;
    public EngineConfig engineConfig;

    private Engine engine;

    private void Start()
    {
        engine = new Engine(engineConfig);
        engineView.Setup(engine);
    }
}
