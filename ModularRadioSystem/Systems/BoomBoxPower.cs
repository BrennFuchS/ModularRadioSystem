using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ModularRadioSystem
{
	internal class BoomBoxPower : MonoBehaviour
	{
		public Speaker speaker;
		public Collider trigger;
		public GameObject channel;
		private bool mouseOver;
		private FsmBool guiUse;
		private FsmString guiInteraction;

		private void Start()
		{
			guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
			guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
		}

		private void Update()
		{
			channel.SetActive(speaker.on);
			if (speaker.raycast.GetHit(trigger))
			{
				guiUse.Value = true;
				guiInteraction.Value = "ON/OFF";
				mouseOver = true;
				if (cInput.GetButtonDown("Use"))
				{
					speaker.on = !speaker.on;
					MasterAudio.PlaySound3DAndForget("CarFoley", transform, false, 0.4f, new float?(1f), 0f, "radio_button");
					return;
				}
			}
			else if (mouseOver)
			{
				guiUse.Value = false;
				guiInteraction.Value = string.Empty;
				mouseOver = false;
			}
		}
	}
}
