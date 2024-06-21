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
    public GameOptions gameOptions;

    public TextMember status;

    private Engine engine;

    public GameEntity game;

    public EngineData engineData;

    private void Start()
    {
        engine = new Engine(engineConfig);
        engine.waiter = Wait;
        engineView.Setup(engine);

        game = engine.CreateEntity("game") as GameEntity;
        game.Setup(gameOptions);
        engine.Evaluate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            engine.Evaluate();
        }

        status.text = $"evaluating {game.isEvaluating}";
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

    [Member]
    public void Reload()
    {
        if (!game.isEvaluating)
        {
            engine.Clear();
            game = engine.CreateEntity("game") as GameEntity;
            game.Setup(gameOptions);
            engine.Evaluate();
        }
    }

    [Member]
    public async void Shuffle()
    {
        if (!game.isEvaluating)
        {
            await game.game.Shuffle();
        }
    }

    [Member]
    public async void TwoColors()
    {
        if (!game.isEvaluating)
        {
            await game.game.TwoColors();
        }
    }

    [Member]
    public void Save()
    {
        if (!game.isEvaluating)
        {
            engineData = engine.Save();
        }
    }

    [Member]
    public void Load()
    {
        if (!game.isEvaluating && engineData != null)
        {
            engine.Load(engineData);
            game = engine.GetEntity<GameEntity>();
        }
    }
}