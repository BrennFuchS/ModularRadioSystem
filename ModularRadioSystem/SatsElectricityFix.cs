using System;
using UnityEngine;

namespace ModularRadioSystem
{
	internal class SatsElectricityFix : MonoBehaviour
	{
		internal RadioElectricity radioElec;

		private void OnEnable() => radioElec.hasPower = true;
		private void OnDisable() => radioElec.hasPower = false;
	}
}
