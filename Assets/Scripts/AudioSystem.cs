using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
	private enum SFX_VALUE
	{
		BOOL = 0,
		CLICK,
		CLOSE,
		DROP1,
		DROP2,
		GAME_OVER,
		SHIFT,
		ITEM,
		ROTATE1,
		ROTATE2,
		SWITCH,
		UNAVAILABLE,
		HARD_DROP1,
		HARD_DROP2,
		HARD_DROP3,
		HARD_DROP4,
		HARD_DROP5,
		MOVE,
		PAUSE,
		CLEAR,
		TETRIS1,
		TETRIS2,
		RESUME,
	}

	private readonly string[] BGM_PATH =
	{
		"BGM/BGM01",
		"BGM/BGM02",
		"BGM/BGM03",
		"BGM/BGM04",
		"BGM/BGM05",
		"BGM/BGM06",
		"BGM/BGM07",
		"BGM/BGM08",
		"BGM/BGM09",
		"BGM/BGM10",
		"BGM/BGM11",
		"BGM/BGM12",
	};
	
	private readonly string[] SFX_PATH =
	{
		"SFX/Bool",
		"SFX/Click",
		"SFX/Close",
		"SFX/Drop1",
		"SFX/Drop2",
		"SFX/GameOver",
		"SFX/Shift",
		"SFX/Item",
		"SFX/Rotate1",
		"SFX/Rotate2",
		"SFX/Switch",
		"SFX/Unavailable",
		"SFX/HardDrop01",
		"SFX/HardDrop02",
		"SFX/HardDrop03",
		"SFX/HardDrop04",
		"SFX/HardDrop05",
		"SFX/Move",
		"SFX/Pause",
		"SFX/Clear",
		"SFX/Tetris1",
		"SFX/Tetris2",
		"SFX/Resume",
	};
	
	private       AudioSource     audioSourceBGM;
	private       AudioSource[]   audioSourcesSFX;
	private       List<AudioClip> bgmSource;
	private       List<AudioClip> sfxSource;
	private       int             sfxIdx;
	private const float           bgmVolumeOrigin = 0.2f;
	private const float           sfxVolume       = 1f;
	private       Coroutine       mainBGM;
	
	public AudioSystem()
	{
		Init();
	}

	private void Init()
	{
		audioSourceBGM             = mainCameraObj.AddComponent<AudioSource>();
		audioSourceBGM.playOnAwake = true;
		audioSourceBGM.loop        = false;
		audioSourceBGM.volume      = bgmVolumeOrigin;
		bgmSource                  = new List<AudioClip>();

		foreach (string path in BGM_PATH)
		{
			bgmSource.Add(Resources.Load<AudioClip>(path));
		}

		mainBGM = StartCoroutine(PlayMainBGM());

		audioSourcesSFX = GridObj.GetComponentsInChildren<AudioSource>();
		sfxIdx          = -1;

		sfxSource = new List<AudioClip>();

		foreach (string path in SFX_PATH)
		{
			sfxSource.Add(Resources.Load<AudioClip>(path));
		}
	}
	
	private IEnumerator PlayMainBGM()
	{
		audioSourceBGM.clip   = bgmSource[0];
		audioSourceBGM.volume = bgmVolumeOrigin;
		audioSourceBGM.pitch  = 1f;
		audioSourceBGM.Play();

		while (true)
		{
			if (!audioSourceBGM.isPlaying)
				audioSourceBGM.Play();

			yield return new WaitForSeconds(2f);
		}
	}

	private void RandomPlayBGM()
	{
		audioSourceBGM.volume = bgmVolumeOrigin;
		audioSourceBGM.clip   = bgmSource[Random.Range(1, bgmSource.Count)];
		audioSourceBGM.Play();
	}

	private IEnumerator PitchDownBGM(float acc)
	{
		const float volDown   = 0.01f;
		const float pitchDown = 0.01f;

		while (audioSourceBGM.volume > 0f)
		{
			audioSourceBGM.volume -= volDown   * acc;
			audioSourceBGM.pitch  -= pitchDown * acc;

			yield return new WaitForSeconds(0.03f);
		}

		audioSourceBGM.Stop();
		audioSourceBGM.volume = bgmVolumeOrigin;
		audioSourceBGM.pitch  = 1f;
	}

	private IEnumerator FadeOutBGM(float acc)
	{
		const float volDown = 0.01f;

		while (audioSourceBGM.volume > 0f)
		{
			audioSourceBGM.volume -= volDown * acc;

			yield return new WaitForSeconds(0.03f);
		}

		audioSourceBGM.volume = 0f;
		audioSourceBGM.Pause();
	}

	private IEnumerator FadeInBGM(float acc)
	{
		const float volUp = 0.01f;

		audioSourceBGM.Play();

		while (audioSourceBGM.volume < bgmVolumeOrigin)
		{
			audioSourceBGM.volume += volUp * acc;

			yield return new WaitForSeconds(0.03f);
		}

		audioSourceBGM.volume = bgmVolumeOrigin;
	}

	private void PauseBGM(float acc)
	{
		StartCoroutine(FadeOutBGM(acc));
	}

	private void ResumeBGM(float acc)
	{
		StartCoroutine(FadeInBGM(acc));
	}

	private IEnumerator PlaySfx(SFX_VALUE value)
	{
		sfxIdx                         = Mathf.Clamp(sfxIdx + 1, 0, audioSourcesSFX.Length - 1);
		audioSourcesSFX[sfxIdx].volume = sfxVolume;
		audioSourcesSFX[sfxIdx].PlayOneShot(sfxSource[(int)value]);

		yield return new WaitForSeconds(1f);

		--sfxIdx;
	}

	private void PlayRandomSfx(SFX_VALUE start, SFX_VALUE end)
	{
		int rand = Random.Range((int)start, (int)end + 1);

		StartCoroutine(PlaySfx((SFX_VALUE)rand));
	}
}