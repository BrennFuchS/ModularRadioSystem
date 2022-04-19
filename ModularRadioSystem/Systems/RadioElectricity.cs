using System;
using UnityEngine;

namespace ModularRadioSystem
{
	public class RadioElectricity : MonoBehaviour
	{
		public bool hasPower;
		public GameObject[] RadioSystemGO;
		public Speaker speaker;
		public MeshRenderer radio;

		private void Update()
		{
			if (hasPower)
			{
				for (int i = 0; i < RadioSystemGO.Length; i++)
					RadioSystemGO[i].SetActive(speaker.volume > 0f && speaker.on);
				if (radio != null)
				{
					radio.material.SetColor("_EmissionColor", (speaker.volume <= 0f || !speaker.on) ? Color.black : Color.white);
					return;
				}
			}
			else
			{
				for (int j = 0; j < RadioSystemGO.Length; j++)
					RadioSystemGO[j].SetActive(false);
				if (radio != null)
					radio.material.SetColor("_EmissionColor", Color.black);
			}
		}
	}
}
