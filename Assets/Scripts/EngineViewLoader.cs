using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        engine.waiter = Wait;

        engineView.Setup(engine);

        game = engine.CreateEntity("game") as GameEntity;
        game.Setup(gameConfig, new GameOptions());

        engine.Evaluate();
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            engine.Evaluate();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            await game.game.Shuffle();
            engine.Evaluate();
        }
    }

    public Task Wait(float time)
    {
        var tcs = new TaskCompletionSource<byte>();
        StartCoroutine(IWait(time, () =>
        {
            tcs.SetResult(0);
        }));
        return tcs.Task;

        IEnumerator IWait(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback.Invoke();
        }
    }
}
