using System;
using UnityEngine;

namespace ModularRadioSystem
{
	public class CDPlayerLCD : MonoBehaviour
	{
		public CDPlayer cdplayer;
		public Tuner tuner;
		public Speaker speaker;
		public TextMesh text;
		private float delay;
		private float lastVol;

		private void Update()
		{
			if (speaker.volume > 0f)
			{
				if (delay >= 0f)
					delay -= Time.deltaTime;

				if (lastVol > speaker.volume || lastVol < speaker.volume)
				{
					lastVol = speaker.volume;
					delay = 0.2f;
				}

				if (delay >= 0f)
				{
					text.text = "VOL " + (speaker.volume * 10f).ToString("00");
					return;
				}

				if (delay < 0f)
				{
					if (tuner.enabled)
					{
						int index = 0;
						float num = float.MaxValue;
						for (int i = 0; i < Tuner.CH.radioStations.Count; i++)
						{
							float num2 = Mathf.Abs(Tuner.CH.radioStations[i].frequency - tuner.frequency);
							if (num2 < num)
							{
								num = num2;
								index = i;
							}
						}
						if (tuner.frequency == Tuner.CH.radioStations[index].frequency)
						{
							text.text = tuner.station.name;
							return;
						}
						text.text = "FM " + tuner.frequency.ToString("0.0");
						return;
					}
					else if (cdplayer.enabled)
					{
						text.text = cdplayer.playText;
						return;
					}
				}
			}
			else if (speaker.volume <= 0f)
				text.text = "";
		}

		private void OnDisable()
		{
			delay = 0f;
			text.text = "";
			lastVol = 0f;
		}
	}
}
