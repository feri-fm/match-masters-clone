using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownHelper : MonoBehaviour
{
    private TMP_Dropdown _dropdown;

    public TMP_Dropdown dropdown => _dropdown ?? (_dropdown = GetComponent<TMP_Dropdown>());

    public int value
    {
        get => dropdown.value;
        set => dropdown.value = value;
    }

    public void SetValueWithoutNotify(int value)
    {
        dropdown.SetValueWithoutNotify(value);
    }

    public void Setup(IEnumerable texts)
    {
        int c = dropdown.value;
        dropdown.SetValueWithoutNotify(0);
        dropdown.ClearOptions();
        var o = new List<TMP_Dropdown.OptionData>();
        foreach (var item in texts)
            o.Add(new TMP_Dropdown.OptionData(item.ToString()));
        dropdown.AddOptions(o);
        dropdown.SetValueWithoutNotify(c);
    }
}
