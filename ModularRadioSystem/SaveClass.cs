using System;

namespace ModularRadioSystem
{
	[Serializable]
	public class SaveClass
	{
		public TunerData[] tunerDatas = new TunerData[0];
		public SpeakerData[] speakerDatas = new SpeakerData[0];
	}
}
