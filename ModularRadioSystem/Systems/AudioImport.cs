using System;
using System.IO;
using AudioLibrary;
using MSCLoader;
using UnityEngine;

namespace ModularRadioSystem
{
	internal class AudioImport
	{
		public static AudioClip LoadAudioFromFile(string path, bool doStream, bool background)
		{
			Stream stream = new MemoryStream(File.ReadAllBytes(path));
			AudioFormat audioFormat = Manager.GetAudioFormat(path);
			string fileName = Path.GetFileName(path);
			if (audioFormat == AudioFormat.unknown)
				ModConsole.Error("Unknown audio format of file " + fileName);

			AudioClip result;
			try
			{
				AudioClip audioClip = Manager.Load(stream, audioFormat, fileName, doStream, background, true);
				float[] array = new float[audioClip.samples * audioClip.channels];
				audioClip.GetData(array, 0);
				for (int i = 0; i < array.Length; i++)
					array[i] *= 0.5f;
				audioClip.SetData(array, 0);
				result = audioClip;
			}
			catch (Exception ex)
			{
				ModConsole.Error(ex.Message);
				if (ModLoader.devMode)
					ModConsole.Error(ex.ToString());
				Console.WriteLine(ex);
				result = null;
			}
			return result;
		}
	}
}
