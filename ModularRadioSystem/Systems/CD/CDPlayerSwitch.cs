using System;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ModularRadioSystem
{
	public class CDPlayerSwitch : MonoBehaviour
	{
		public CDPlayer cdplayer;
		public Tuner tuner;
		public Speaker speaker;
		public InteractionRaycast raycast;
		public Collider trigger;
		private FsmBool guiUse;
		private FsmString guiInteraction;
		private bool mouseOver;

		private void Start()
		{
			raycast = Resources.FindObjectsOfTypeAll<InteractionRaycast>().First<InteractionRaycast>();
			guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
			guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
		}

		private void Update()
		{
			if (raycast.GetHit(trigger))
			{
				guiUse.Value = true;
				guiInteraction.Value = "RADIO / CD";
				mouseOver = true;
				if (Input.GetMouseButtonDown(0))
				{
					tuner.enabled = !tuner.enabled;
					cdplayer.enabled = !tuner.enabled;
					MasterAudio.PlaySound3DAndForget("CarFoley", transform, false, 0.4f, new float?(1f), 0f, "cd_button");
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
