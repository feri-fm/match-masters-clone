using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class EditorAdManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject panel;
    public GameObject loading;
    public GameObject display;

    public static EditorAdManager instance { get; private set; }

    public UnityAction reward = () => { };
    public UnityAction error = () => { };

    private void Awake()
    {
        instance = this;
        panel.SetActive(false);
        videoPlayer.Stop();
    }

    public void Show()
    {
        StartCoroutine(IShow());
    }

    private IEnumerator IShow()
    {
        panel.SetActive(true);
        display.SetActive(false);
        loading.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        display.SetActive(true);
        loading.SetActive(false);
        videoPlayer.Play();
    }

    public void Reward()
    {
        panel.SetActive(false);
        videoPlayer.Stop();
        reward.Invoke();
    }

    public void Skip()
    {
        panel.SetActive(false);
        videoPlayer.Stop();
        error.Invoke();
    }
}
