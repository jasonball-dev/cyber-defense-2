using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveNotification : MonoBehaviour
{
    public Text waveText;
    public float displayTime = 2f;

    private void Start()
    {
        waveText.gameObject.SetActive(false);
    }

    public void ShowMessage(string textIn, float duration = 2)
    {
        waveText.text = textIn;
        displayTime = duration;
        StartCoroutine(DisplayText());
    }

    private IEnumerator DisplayText()
    {
        waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        waveText.gameObject.SetActive(false);
        displayTime = 2f;
    }
}