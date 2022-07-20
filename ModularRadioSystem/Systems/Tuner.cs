using System;
using System.Collections;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ModularRadioSystem
{
	public class Tuner : MonoBehaviour
	{
		public static RadioChannels CH = RadioChannels.GetInstance();
		public RadioStation station;
		public Speaker speaker;
		public float frequency = 76f;
		public InteractionRaycast raycast;
		public Collider trigger;
		public Transform knob;
		public Vector3 knobAxis;
		public float knobMult;
		public bool hasKnob;
		private string mouseWheel = "Mouse ScrollWheel";
		private float delay;
		private float holdTime;
		private bool mouseOver;
		private bool searchTriggered;
		private bool holding;
		private bool right;
		public string currentClip;
		public AudioClip copiedClip;
		private FsmBool guiUse;
		private FsmString guiInteraction;

		private void Start()
		{
			raycast = Resources.FindObjectsOfTypeAll<InteractionRaycast>().First();
			guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
			guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
		}

		private void OnEnable()
		{
			SetRadio();
		}

		private void OnDisable()
		{
			speaker.channelVolume = 1f;
			speaker.staticVolume = 0f;
			speaker.SC.Stop();
		}

		private void Update()
		{
			SetRadio();
			delay -= Time.deltaTime;
			if (raycast.GetHit(trigger) && !holding)
			{
				guiUse.Value = true;
				guiInteraction.Value = "TUNER (FM " + frequency.ToString("0.0") + ")";
				mouseOver = true;
				if (hasKnob)
				{
					if (frequency < 160f && Input.GetAxis(mouseWheel) < 0f && delay <= 0f)
						ChangeFreq(false);
					else if (frequency > 55f && Input.GetAxis(mouseWheel) > 0f && delay <= 0f)
						ChangeFreq(true);
				}
				else if (!speaker.isCD)
				{
					if (cInput.GetButtonDown("Use") && !searchTriggered)
					{
						holding = true;
						MasterAudio.PlaySound3DAndForget("CarFoley", transform, false, 0.4f, 1f, 0f, "radio_button");
					}
				}
				else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !searchTriggered)
				{
					holding = true;
					right = Input.GetMouseButton(1);
					MasterAudio.PlaySound3DAndForget("CarFoley", transform, false, 0.4f, 1f, 0f, "cd_button");
				}
			}
			else if (mouseOver)
			{
				guiUse.Value = false;
				guiInteraction.Value = string.Empty;
				mouseOver = false;
			}
			if (holding)
			{
				holdTime += Time.deltaTime;
				if ((!speaker.isCD && !cInput.GetButton("Use")) || (speaker.isCD && !Input.GetMouseButton(0) && !Input.GetMouseButton(1)))
				{
					if (holdTime < 1.5f)
						ChangeFreq(speaker.isCD && right);
					holding = false;
					holdTime = 0f;
				}
				else if (holdTime > 1.5f)
				{
					holding = false;
					searchTriggered = true;
					StartCoroutine(Search(speaker.isCD && !Input.GetMouseButton(0)));
					holdTime = 0f;
				}
			}
			if (hasKnob)
				knob.localEulerAngles = knobAxis * (frequency - 75f) * knobMult;
		}

		public void ChangeFreq(bool negative = false)
		{
			if (!hasKnob)
			{
				if (negative)
				{
					if (frequency - 0.1f < 75f)
						frequency = 92.5f;
				}
				else if (frequency + 0.1f > 92.5f)
					frequency = 75f;
			}
			frequency = (float)Math.Round((double)Mathf.Clamp(frequency + (negative ? -0.1f : 0.1f), 75f, 92.5f), 1);
			delay = 0.025f;
			if (hasKnob)
				MasterAudio.PlaySound3DAndForget("CarFoley", transform, false, 0.4f, new float?(1f), 0f, "analog_radio_button");
		}

		private void UpdateAudio()
		{
			if (!RadioChannels.allowPlaying)
				return;

			if (copiedClip == null)
			{
				if (station.musicPath.Length > 4)
					copiedClip = station.clip;
				else
					copiedClip = station.clip;
				if (speaker.SC.gameObject.activeInHierarchy && speaker.SC.isPlaying)
				{
					speaker.SC.clip = station.clip;
					currentClip = station.clip.name;
					speaker.SC.Play();
					speaker.SC.time = station.time;
					return;
				}
			}
			else
			{
				if (speaker.SC.gameObject.activeInHierarchy)
				{
					if (!speaker.SC.isPlaying)
					{
						speaker.Init();
                        speaker.SC.clip = copiedClip;
						currentClip = station.clip.name;
						speaker.SC.Play();
						speaker.SC.time = station.time;
					}
					else
					{
						if (currentClip != station.clip.name)
						{
							speaker.SC.clip = copiedClip;
							currentClip = station.clip.name;
							speaker.SC.Play();
							speaker.SC.time = station.time;
						}
						if (speaker.SC.time > station.time + 0.5f || speaker.SC.time < station.time - 0.5f)
							speaker.SC.time = station.time;
					}
				}
				if (copiedClip.name != station.clip.name)
					copiedClip = null;
			}
		}

		private void SetRadio()
		{
			int index = 0;
			float num = float.MaxValue;
			for (int i = 0; i < CH.radioStations.Count; i++)
			{
				float num2 = Mathf.Abs(CH.radioStations[i].frequency - frequency);
				if (num2 < num)
				{
					num = num2;
					index = i;
				}
			}
			station = Tuner.CH.radioStations[index];
			float num3 = (float)Math.Round(Mathf.Clamp(frequency - station.frequency, -0.3f, 0.3f), 1);
			if (num3 >= -0.3f && num3 <= 0.3f)
			{
				speaker.channelVolume = 1f - Mathf.Abs(num3 * 3.3333333f);
				speaker.staticVolume = 1f * Mathf.Abs(num3 * 3.3333333f);
				UpdateAudio();
			}
		}

		private IEnumerator Search(bool negative = false)
		{
			ChangeFreq(negative);
			while (searchTriggered)
			{
				int index = 0;
				float num = float.MaxValue;
				for (int i = 0; i < CH.radioStations.Count; i++)
				{
					float num2 = Mathf.Abs(CH.radioStations[i].frequency - frequency);
					if (num2 < num)
					{
						num = num2;
						index = i;
					}
				}
				if (frequency != CH.radioStations[index].frequency)
					ChangeFreq(negative);
				else
					searchTriggered = false;
				yield return new WaitForSeconds(0.05f);
			}
			yield return null;
			yield break;
		}
	}
}
