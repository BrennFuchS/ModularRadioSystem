using System;
using UnityEngine;

namespace ModularRadioSystem
{
	internal class SatsSpeaker : MonoBehaviour
	{
		internal Speaker speaker;

		private void LateUpdate()
		{
			if (speaker.hasSubwoofers != (transform.parent.parent.name == "SpeakerBass"))
				speaker.hasSubwoofers = (transform.parent.parent.name == "SpeakerBass");
		}
	}
}
