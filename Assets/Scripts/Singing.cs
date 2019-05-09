using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Singing : MonoBehaviour
{
	private const float FADE_WAIT_TIME = 1;
	private const float FADE_OUT_TIME = 1;
	private const float FADE_IN_TIME = 0.5f;

	public Image songImage;

	private Coroutine crtFadeIn;
	private Coroutine crtFadeOut;

    private void Start()
    {
		SetColor(0);
    }
	
    private void Update()
    {
		float inputX = Input.GetAxis("SongHorizontal");
		float inputY = Input.GetAxis("SongVertical");
		if (inputX == 0 && inputY == 0)
		{
			if (crtFadeIn != null) StopCoroutine(crtFadeIn);
			if (crtFadeOut == null) crtFadeOut = StartCoroutine(FadeOut());
		}
		else
		{
			if (crtFadeOut != null) StopCoroutine(crtFadeOut);
			if (crtFadeIn == null) crtFadeIn = StartCoroutine(FadeIn());
		}
    }

	private IEnumerator FadeOut()
	{
		yield return new WaitForSeconds(FADE_WAIT_TIME);
		for (float t = 0; t < FADE_OUT_TIME; t += Time.deltaTime)
		{
			SetColor(1 - (t / FADE_OUT_TIME));
			yield return null;
		}
		SetColor(0);
		crtFadeOut = null;
	}

	private IEnumerator FadeIn()
	{
		for (float t = 0; t < FADE_IN_TIME; t += Time.deltaTime)
		{
			SetColor(t / FADE_IN_TIME);
			yield return null;
		}
		SetColor(1);
		crtFadeIn = null;
	}

	private void SetColor(float a)
	{
		songImage.color = new Color(1, 1, 1, a);
	}
}
