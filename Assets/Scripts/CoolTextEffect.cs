using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolTextEffect : MonoBehaviour
{
    public TextAdaptor text;
    public float time;
    public int rate = 1;

    private float lastTime;

    private void Update()
    {
        if (Time.time > lastTime + time)
        {
            lastTime = Time.time;
            for (int i = 0; i < rate; i++)
            {
                Apply();
            }
        }
    }

    private void Apply()
    {
        var r = Random.Range(1, text.text.Length);
        if (text.text[r] == ' ') r -= 1;
        var chars = "abcdefghijklmnopqrstuvwxyz";
        var ch = chars[Random.Range(0, chars.Length)];
        text.text = text.text.Substring(0, r) + ch + text.text.Substring(r + 1);
    }
}
