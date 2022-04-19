using System;

namespace ModularRadioSystem
{
	// Token: 0x02000014 RID: 20
	[Serializable]
	public class TunerData
	{
		// Token: 0x0400006E RID: 110
		public string vehicle;

		// Token: 0x0400006F RID: 111
		public bool radioType;

		// Token: 0x04000070 RID: 112
		public bool isActive = true;

		// Token: 0x04000071 RID: 113
		public float tune;
	}
}
