using System.Collections.Generic;
using MSCLoader;
using UnityEngine;

namespace ModularRadioSystem
{
	public class RadioStation : MonoBehaviour
	{
		[Header("the name of your radio station. should not be longer than 10 letters.")]
		public string name;
		[Header("the Frequency of your radio station. Range 75 to 92.5")]
		[Range(75f, 92.5f)]
		public float frequency = 80f;
		public string musicPath;
		public List<object> music = new List<object>();
		public List<object> ads = new List<object>();
		private int titlesPlayed;
		private int titlesToAd;
		private bool adPlayed = true;
		private bool hasAds;
		public AudioClip clip;
		public float time;
		public float startTime;

		private void Start()
		{
			hasAds = ads.Count > 0;
		}

		private void Update()
		{
			if (RadioChannels.allowPlaying)
			{
				if (clip == null)
				{
					if (!adPlayed)
					{
						object obj = ads[Random.Range(0, ads.Count - 1)];
						if (obj is AudioClip)
						{
							clip = (AudioClip)obj;
						}
						else if (obj is string)
						{
							clip = AudioImport.LoadAudioFromFile((string)obj, false, true);
						}
						time = 0f;
						startTime = Time.time;
						adPlayed = true;
					}
					else if (adPlayed || !hasAds)
					{
						object obj2 = music[Random.Range(0, music.Count - 1)];
						if (obj2 is AudioClip)
						{
							clip = (AudioClip)obj2;
						}
						else if (obj2 is string)
						{
							clip = AudioImport.LoadAudioFromFile((string)obj2, false, true);
							ModConsole.Print("MRS : Loaded Clip " + clip.name + " for " + name);
						}
						time = 0f;
						titlesPlayed++;
						startTime = Time.time;
					}
				}
				else
				{
					time = Time.time - startTime;
					if (time >= clip.length)
					{
						if (!music.Contains(clip) && !ads.Contains(clip))
						{
							Object.Destroy(clip);
							ModConsole.Print("MRS : Freed memory from Clip " + clip.name + " in " + name);
						}
						clip = null;
					}
				}
				if (hasAds && adPlayed && titlesPlayed >= titlesToAd)
				{
					titlesToAd = Random.Range(0, music.Count - 1);
					titlesPlayed = 0;
					adPlayed = false;
				}
			}
		}
	}
}
