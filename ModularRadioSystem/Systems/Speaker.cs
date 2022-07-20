using System;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ModularRadioSystem
{
	public class Speaker : MonoBehaviour
	{
		public AudioSource SS;
		public AudioSource SC;
		public InteractionRaycast raycast;
		public Collider trigger;
		public Transform knob;
		public Vector3 knobAxis;
		public float knobMult = -300f;
		public string interaction = "ON/OFF/VOLUME";
		private string mouseWheel = "Mouse ScrollWheel";
		private float delay;
		public bool hasSubwoofers;
		public bool isCD;
		public bool on;
		public bool canTurnOff = true;
		public AudioHighPassFilter[] ahpf;
		public AudioLowPassFilter[] alpf;
		private FsmBool guiUse;
		private FsmString guiInteraction;
		public float volume;
		public float staticVolume;
		public float channelVolume;
		private bool mouseOver;

		private void OnDisable()
		{
			SS.Stop();
			SC.Stop();
		}

		private void OnEnable()
		{
			SS.Play();
		}

		internal void Init()
        {
            ahpf[0].enabled = !hasSubwoofers;
            ahpf[1].enabled = !hasSubwoofers;
            alpf[0].enabled = hasSubwoofers;
            alpf[1].enabled = hasSubwoofers;
        }

		private void Start()
		{
			raycast = Resources.FindObjectsOfTypeAll<InteractionRaycast>().First<InteractionRaycast>();
			guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
			guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
		}

		private void Update()
		{
            delay -= Time.deltaTime;
			if (raycast.GetHit(trigger))
			{
				guiUse.Value = true;
				guiInteraction.Value = interaction;
				mouseOver = true;
				if (Input.GetMouseButtonDown(0) && canTurnOff)
				{
					on = !on;
					if (!isCD)
						MasterAudio.PlaySound3DAndForget("CarFoley", transform, false, 0.4f, new float?(1f), 0f, "radio_button");
					else if (volume > 0f && on)
						MasterAudio.PlaySound3DAndForget("CarFoley", transform, false, 0.4f, new float?(1f), 0f, "cd_button");
				}
				if (volume < 1f && Input.GetAxis(mouseWheel) < 0f && delay <= 0f)
				{
					volume = (float)Math.Round((double)(volume + 0.1f), 1);
					Sound();
				}
				else if (volume > 0f && Input.GetAxis(mouseWheel) > 0f && delay <= 0f)
				{
					volume = (float)Math.Round((double)(volume - 0.1f), 1);
					Sound();
				}
			}
			else if (mouseOver)
			{
				guiUse.Value = false;
				guiInteraction.Value = string.Empty;
				mouseOver = false;
			}
			if (knob != null)
			{
				knob.localEulerAngles = knobAxis * volume * knobMult;
			}
			UpdateVolume();
		}

		private void Sound()
		{
			if (!isCD)
				MasterAudio.PlaySound3DAndForget("CarFoley", transform, false, 0.4f, 1f, 0f, "analog_radio_button");
			else if (volume > 0f && on)
				MasterAudio.PlaySound3DAndForget("CarFoley", transform, false, 0.4f, 1f, 0f, "cd_button");
			delay = 0.05f;
		}

		private void UpdateVolume()
		{
			SS.volume = staticVolume * volume;
			SC.volume = channelVolume * volume;
		}
	}
}
