using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Singing : MonoBehaviour
{
	private const float FADE_WAIT_TIME = 1;
	private const float FADE_OUT_TIME = 1;
	private const float FADE_IN_TIME = 0.5f;

	private const float CURSOR_RADIUS = 80;
	
	//private static int[] NOTES = new int[] { 0, 2, 4, 5, 7, 9, 11, 12 }; // c major
	//private static int[] NOTES = new int[] { -6, -4, -3, -1, 1, 2, 4, 6 }; // b flat minor
	//private static int[] NOTES = new int[] { -2, 0, 1, 3, 5, 6, 8, 10 }; // d flat minor
	private static int[] NOTES = new int[] { -8, -6, -5, -3, -1, 0, 2, 4 }; // e minor

	private Dictionary<int[], Action> songs = new Dictionary<int[], Action>();

	public Image songImage;
	public Image cursorImage;
	public GameObject notePrefab;
	public int transpose;
	public int whaleTranspose;
	public GameObject breakParticles;

	private int lastNote = -1;
	private List<int> playedNotes = new List<int>();

	private Coroutine crtFadeIn;
	private Coroutine crtFadeOut;

    private void Start()
    {
		SetColor(0);

		songs.Add(new int[] { 0, 3, 2 }, BreakEffect);
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
		
		if (rS > 0.8f * CURSOR_RADIUS)
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
		int note = NOTES[n];
		float pitch = Mathf.Pow(2, (note + transpose) / 12f);

		GameObject noteObj = Instantiate(notePrefab);
		Note noteScript = noteObj.GetComponent<Note>();
		Destroy(noteObj, 5);

		noteScript.noteSrc.pitch = pitch;
		noteScript.noteSrc.Play();

		float whalePitch = Mathf.Pow(2, (note + whaleTranspose) / 12f);
		noteScript.whaleSrc.pitch = whalePitch;
		noteScript.whaleSrc.Play();

		playedNotes.Add(n);
		CheckSongs();
	}

	private void CheckSongs()
	{
		foreach(KeyValuePair<int[], Action> song in songs)
		{
			CheckSong(song.Key, song.Value);
		}
	}

	private bool CheckSong(int[] song, Action effect)
	{
		if (playedNotes.Count < song.Length)
		{
			return false;
		}

		int start = playedNotes.Count - song.Length;

		for (int i = 0; i < song.Length; i++)
		{
			if (playedNotes[start + i] != song[i])
			{
				return false;
			}
		}

		effect();
		return true;
	}

	private void BreakEffect()
	{
		print("BOOM");
		RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, 20);
		if (hits.Length > 0) {
			RaycastHit hit = hits.OrderBy(h => h.distance).First();
			/*IEnumerable orderedHits = hits.OrderBy(h => h.distance);
			foreach (RaycastHit hit in orderedHits)
			{*/
			GameObject other = hit.collider.gameObject;
			if (other.CompareTag("Breakable"))
			{
				Instantiate(breakParticles, other.transform.position, Quaternion.identity);
				Destroy(other);
			}
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
		Color c = new Color(1, 1, 1, a);
		songImage.color = c;
		cursorImage.color = c;
	}
}
