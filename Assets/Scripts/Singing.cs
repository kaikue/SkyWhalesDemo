using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Singing : MonoBehaviour
{
	private const float FADE_WAIT_TIME = 1;
	private const float FADE_OUT_TIME = 1;
	private const float FADE_IN_TIME = 0.5f;

	private const float CURSOR_RADIUS = 80;

	private static int[] NOTES = new int[] { 0, 2, 4, 5, 7, 9, 11, 12 };

	public Image songImage;
	public Image cursorImage;
	public AudioSource noteSrc;

	private int lastNote = -1;

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
			/*if (crtFadeIn != null) StopCoroutine(crtFadeIn);
			if (crtFadeOut == null) crtFadeOut = StartCoroutine(FadeOut());*/
			SetColor(0);
			lastNote = -1;
		}
		else
		{
			/*if (crtFadeOut != null) StopCoroutine(crtFadeOut);
			if (crtFadeIn == null) crtFadeIn = StartCoroutine(FadeIn());*/
			SetColor(1);
			MoveCursor(inputX, inputY);
		}
    }

	private void MoveCursor(float inputX, float inputY)
	{
		float theta = Mathf.Atan2(inputY, inputX);
		float r = Mathf.Sqrt(inputX * inputX + inputY * inputY);
		float rS = Mathf.Clamp(r, 0, 1) * CURSOR_RADIUS;
		float x = rS * Mathf.Cos(theta);
		float y = rS * Mathf.Sin(theta);
		cursorImage.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
		//cursorImage.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(inputX * CURSOR_RADIUS, inputY * CURSOR_RADIUS, 0);
		//cursorImage.transform.position = new Vector3(inputX * CURSOR_RADIUS, inputY * CURSOR_RADIUS, 0);
		
		if (rS == CURSOR_RADIUS)
		{
			PlayNoteAngle(theta);
		}
	}

	private void PlayNoteAngle(float theta)
	{
		int n = (int)Mathf.Round(theta / (Mathf.PI / 4));
		int n2 = 7 - (n + 5) % 8;
		PlayNote(n2);
	}

	private void PlayNote(int n)
	{
		if (lastNote == n) return;

		lastNote = n;
		float transpose = -4;
		int note = NOTES[n];
		float pitch = Mathf.Pow(2, (note + transpose) / 12f);
		noteSrc.pitch = pitch;
		noteSrc.Play();
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
		Color c = new Color(1, 1, 1, a);
		songImage.color = c;
		cursorImage.color = c;
	}
}
