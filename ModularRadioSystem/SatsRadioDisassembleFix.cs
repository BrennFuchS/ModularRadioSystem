using System;
using UnityEngine;

namespace ModularRadioSystem
{
	internal class SatsRadioDisassembleFix : MonoBehaviour
	{
		internal Transform sats;
		internal RadioElectricity radioElec;

		private void Update()
		{
			if (radioElec.transform.root != sats)
				radioElec.hasPower = false;
		}
	}
}
