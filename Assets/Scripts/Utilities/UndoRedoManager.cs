using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UndoRedoManager : MonoBehaviour
{
    public bool merge;
    public GameObject[] btnUndo;
    public GameObject[] btnRedo;

    public List<UndoRedoAction> actions = new List<UndoRedoAction>();
    private int status = -1;

    public bool canUndo => status >= 0;
    public bool canRedo => actions.Count > status + 1;

    public UndoRedoAction lastAction => actions.Count > 0 ? actions[actions.Count - 1] : null;
    public UndoRedoAction undoAction => actions.Count > 0 && status >= 0 ? actions[status] : null;
    public UndoRedoAction redoAction => actions.Count > 0 && status + 1 >= 0 && status + 1 < actions.Count ? actions[status + 1] : null;

    void Start()
    {
        SetupUndoRedo();
    }

    public void AddAction(string undoTitle, UnityAction undoAction, string redoTitle, UnityAction redoAction)
    {
        AddAction(new UndoRedoAction(
            new EditorAction(undoTitle, undoAction),
            new EditorAction(redoTitle, redoAction)
        ));
    }

    public void AddAction(UndoRedoAction a)
    {
        actions = actions.GetRange(0, Mathf.Max(0, status + 1));
        actions.Add(a);
        status++;
        SetupUndoRedo();
    }

    public void Undo()
    {
        if (canUndo)
        {
            var time = actions[status].time;
            do
            {
                actions[status].Undo();
                Alert(actions[status].undo[0].title);
                status--;
            }
            while (merge && canUndo && time == actions[status].time);
        }
        SetupUndoRedo();
    }
    public void Redo()
    {
        if (canRedo)
        {
            var time = actions[status + 1].time;
            do
            {
                status++;
                actions[status].Redo();
                Alert(actions[status].redo[0].title);
            }
            while (merge && status + 1 < actions.Count && time == actions[status + 1].time);
        }
        SetupUndoRedo();
    }

    public void SetupUndoRedo()
    {
        foreach (var item in btnUndo)
            item.SetActive(canUndo);
        foreach (var item in btnRedo)
            item.SetActive(canRedo);
    }
    public void Clear()
    {
        status = -1;
        actions.Clear();
        SetupUndoRedo();
    }

    public void Alert(string message)
    {

    }
}

[System.Serializable]
public class UndoRedoAction
{
    public float time;
    public List<EditorAction> undo = new List<EditorAction>();
    public List<EditorAction> redo = new List<EditorAction>();

    public string undoTitle => undo[0].title;
    public string redoTitle => redo[0].title;

    public UndoRedoAction()
    {
        time = Time.time;
    }
    public UndoRedoAction(EditorAction undo, EditorAction redo)
    {
        time = Time.time;
        this.undo.Add(undo);
        this.redo.Add(redo);
    }

    public void AddUndoAction(UnityAction action)
    {
        AddUndoAction(new EditorAction(undo[0].title, action));
    }
    public void AddUndoAction(EditorAction action)
    {
        undo.Add(action);
        action.title = undo[0].title;
    }
    public void AddRedoAction(UnityAction action)
    {
        AddRedoAction(new EditorAction(redo[0].title, action));
    }
    public void AddRedoAction(EditorAction action)
    {
        redo.Add(action);
        action.title = redo[0].title;
    }

    public void Undo()
    {
        foreach (var item in undo)
            item?.Invoke();
    }
    public void Redo()
    {
        foreach (var item in redo)
            item?.Invoke();
    }
}

[System.Serializable]
public class EditorAction
{
    public string title;
    public UnityAction action;

    public EditorAction(string title, UnityAction action)
    {
        this.title = title;
        this.action = action;
    }

    public void Invoke()
    {
        action.Invoke();
    }
}
