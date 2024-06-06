using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Awaiter : MonoBehaviour
{
    public static Awaiter instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Coroutine AwaitYield(YieldInstruction yieldInstruction, UnityAction done)
        => StartCoroutine(_AwaitYield(yieldInstruction, done));
    private IEnumerator _AwaitYield(YieldInstruction yieldInstruction, UnityAction done)
    {
        yield return yieldInstruction;
        done.Invoke();
    }

    public Coroutine AwaitYield(CustomYieldInstruction yieldInstruction, UnityAction done)
        => StartCoroutine(_AwaitYield(yieldInstruction, done));
    private IEnumerator _AwaitYield(CustomYieldInstruction yieldInstruction, UnityAction done)
    {
        yield return yieldInstruction;
        done.Invoke();
    }
}

public static class AwaiterExtensions
{
    public static TaskAwaiter<int> GetAwaiter(this YieldInstruction yieldInstruction)
        => CreateAwaiter((res) => Awaiter.instance.AwaitYield(yieldInstruction, res));

    public static TaskAwaiter<int> GetAwaiter(this CustomYieldInstruction yieldInstruction)
        => CreateAwaiter((res) => Awaiter.instance.AwaitYield(yieldInstruction, res));

    public static TaskAwaiter<int> GetAwaiter(this int wait)
        => CreateAwaiter((res) => Awaiter.instance.AwaitYield(new WaitForSeconds(wait), res));

    public static TaskAwaiter<int> GetAwaiter(this float wait)
        => CreateAwaiter((res) => Awaiter.instance.AwaitYield(new WaitForSeconds(wait), res));

    public static TaskAwaiter<T> GetAwaiter<T>(this Promise<T> promise)
    {
        var tcs = new TaskCompletionSource<T>();
        promise.body(tcs.SetResult, tcs.SetException);
        return tcs.Task.GetAwaiter();
    }

    public static TaskAwaiter<int> CreateAwaiter(UnityAction<UnityAction> action)
    {
        var tcs = new TaskCompletionSource<int>();
        action.Invoke(() =>
        {
            tcs.SetResult(0);
        });
        return tcs.Task.GetAwaiter();
    }
}

public class Promise<T>
{
    public UnityAction<UnityAction<T>, UnityAction<Exception>> body { get; }

    public Promise(UnityAction<UnityAction<T>, UnityAction<Exception>> body)
    {
        this.body = body;
    }
    public Promise(UnityAction<UnityAction<T>> body)
    {
        this.body = (r, e) => body((t) => r(t));
    }
}

public class Promise : Promise<int>
{
    public Promise(UnityAction<UnityAction, UnityAction<Exception>> body)
        : base((r, e) => body(() => r(0), ee => e(ee))) { }

    public Promise(UnityAction<UnityAction> body)
        : base((r, e) => body(() => r(0))) { }
}
