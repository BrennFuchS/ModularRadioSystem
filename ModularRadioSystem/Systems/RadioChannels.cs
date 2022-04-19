using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModularRadioSystem
{
	public class RadioChannels : MonoBehaviour
	{
		public static bool allowPlaying;
		public List<RadioStation> radioStations = new List<RadioStation>();

		public static RadioChannels GetInstance() => Resources.FindObjectsOfTypeAll<RadioChannels>().First();

		public static RadioStation AddChannel(float frequency, string name, string musicPath, IEnumerable<object> music, IEnumerable<object> ads = null)
		{
			RadioChannels instance = GetInstance();
			GameObject gameObject = new GameObject(name.ToUpper());
			RadioStation radioStation = gameObject.AddComponent<RadioStation>();
			radioStation.name = name;
			radioStation.frequency = frequency;
			radioStation.musicPath = musicPath;
			if (music != null)
			{
				radioStation.music = music.ToList();
			}
			if (ads != null)
			{
				radioStation.ads = ads.ToList();
			}
			instance.radioStations.Add(radioStation);
			gameObject.transform.SetParent(instance.transform);
			return radioStation;
		}
	}
}
