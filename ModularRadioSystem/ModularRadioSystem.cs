using System.Collections.Generic;
using System.IO;
using System.Linq;
using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace ModularRadioSystem
{
	public class ModularRadioSystem : Mod
	{
		public override string ID => "ModularRadioSystem";
		public override string Name => "MRS";
		public override string Author => "BrennFuchS";
		public override string Version => "1.0.0";
		public override string Description => "Replaces the terrible in-game Radio System with one supporting Custom Channels!";

		public static GameObject RadioPrefab;
		public static GameObject CDPlayerPrefab;
		public static GameObject boomboxPrefab;
		private string mainPath = Path.GetFullPath(".") + "\\";
		private string customRadioPath;
		private SaveClass save;

		public override void ModSetup()
        {
			SetupFunction(Setup.OnNewGame, Mod_OnNewGame);
			SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);
			SetupFunction(Setup.OnLoad, Mod_OnLoad);
			SetupFunction(Setup.OnSave, Mod_OnSave);
			SetupFunction(Setup.PostLoad, Mod_SecondPassOnLoad);
		}

        void Mod_OnNewGame()
		{
			SaveLoad.SerializeSaveFile<SaveClass>(this, new SaveClass(), "MRS");
		}
		void Mod_OnMenuLoad()
		{
			customRadioPath = Path.GetFullPath(".") + "\\MRS Stations";
			if (!Directory.Exists(customRadioPath)) 
				Directory.CreateDirectory(customRadioPath);
			if (!File.Exists(customRadioPath + "\\ExampleFM,78.5.zip") || File.ReadAllBytes(customRadioPath + "\\ExampleFM,78.5.zip") != Properties.Resources.ExampleFM_78_5)
				File.WriteAllBytes(customRadioPath + "\\ExampleFM,78.5.zip", Properties.Resources.ExampleFM_78_5);
		}
		void Mod_OnLoad()
		{
			AssetBundle assetBundle = AssetBundle.CreateFromMemoryImmediate(Properties.Resources.mrs);
			GameObject gameObject = assetBundle.LoadAsset<GameObject>("RadioSystem.prefab");
			GameObject.Instantiate<GameObject>(gameObject);
			GameObject.Destroy(gameObject);
			RadioPrefab = assetBundle.LoadAsset<GameObject>("Radio.prefab");
			CDPlayerPrefab = assetBundle.LoadAsset<GameObject>("cd player.prefab");
			boomboxPrefab = assetBundle.LoadAsset<GameObject>("boombox.prefab");
			assetBundle.Unload(false);
			CreateFolk();
			CreateChannel1();
			CreateCustomChannels();
			ApplySatsumaStockRadio();
		}
		void Mod_OnSave()
		{
			save = new SaveClass();

			List<TunerData> list = new List<TunerData>();
			foreach (Tuner tuner in Resources.FindObjectsOfTypeAll<Tuner>())
			{
				if (!tuner.transform.root.IsPrefab())
				{
					string name = tuner.transform.root.name;
					bool hasKnob = tuner.hasKnob;
					float frequency = tuner.frequency;
					list.Add(new TunerData
					{
						vehicle = name,
						tune = frequency,
						radioType = hasKnob,
						isActive = tuner.enabled
					});
				}
			}
			save.tunerDatas = list.ToArray();

			List<SpeakerData> list2 = new List<SpeakerData>();
			foreach (Speaker speaker in Resources.FindObjectsOfTypeAll<Speaker>())
			{
				if (!speaker.transform.root.IsPrefab())
				{
					string name2 = speaker.transform.root.name;
					bool on = speaker.on;
					float volume = speaker.volume;
					list2.Add(new SpeakerData
					{
						vehicle = name2,
						isOn = on,
						volume = volume
					});
				}
			}
			save.speakerDatas = list2.ToArray();

			SaveLoad.SerializeSaveFile<SaveClass>(this, save, "MRS");
		}
		void Mod_SecondPassOnLoad()
		{
			Tuner.CH = RadioChannels.GetInstance();
			ReplaceStockRadios();
			ReplacePortableRadios();
			GameObject gameObject = GameObject.Instantiate<GameObject>(CDPlayerPrefab);
			gameObject.transform.position = new Vector3(-2f, 0f, 0f);
			gameObject.name = "cdplayer(Bruhx)";
			ApplySave();
			RadioChannels.allowPlaying = true;
		}

		private void ApplySave()
		{
			save = SaveLoad.DeserializeSaveFile<SaveClass>(this, "MRS");
			if (save == null)
				save = new SaveClass();
			foreach (Tuner tuner in Resources.FindObjectsOfTypeAll<Tuner>())
			{
				if (!tuner.transform.root.IsPrefab())
				{
					string name = tuner.transform.root.name;
					float frequency = (float)System.Math.Round(Random.Range(75f, 92.5f), 1);
					if (save.tunerDatas.Length != 0)
					{
						for (int j = 0; j < save.tunerDatas.Length; j++)
						{
							if (save.tunerDatas[j].vehicle == name)
							{
								if (save.tunerDatas[j].radioType == tuner.hasKnob)
									frequency = save.tunerDatas[j].tune;
								tuner.enabled = save.tunerDatas[j].isActive;
							}
						}
					}
					tuner.frequency = frequency;
				}
			}
			foreach (Speaker speaker in Resources.FindObjectsOfTypeAll<Speaker>())
			{
				if (!speaker.transform.root.IsPrefab())
				{
					string name2 = speaker.transform.root.name;
					if (save.speakerDatas.Length != 0)
					{
						for (int k = 0; k < save.tunerDatas.Length; k++)
						{
							if (save.speakerDatas[k].vehicle == name2)
							{
								speaker.volume = save.speakerDatas[k].volume;
								speaker.on = save.speakerDatas[k].isOn;
							}
						}
					}
				}
			}
		}

		private void ReplaceStockRadios()
		{
			if (save == null)
				save = new SaveClass();

			List<GameObject> list = 
				Resources.FindObjectsOfTypeAll<GameObject>()
				.Where(x => x.name.StartsWith("RadioPivot"))
				.Where(x => x.transform.root.name != "JAIL")
				.ToList();

			ModConsole.Log("<color=purple>MRS : Found " + list.Count.ToString() + " Radios.</color>");

			foreach (GameObject gameObject in list)
			{
				ModConsole.Log("<color=purple>MRS : Replacing Radio in Vehicle: " + gameObject.transform.root.name + "...</color>");
				GameObject gameObject2 = GameObject.Instantiate<GameObject>(ModularRadioSystem.RadioPrefab);
				Transform transform = gameObject.transform.Find("Radio");
				gameObject2.transform.SetParent(transform.transform, false);
				gameObject2.transform.localEulerAngles = Vector3.zero;
				Transform transform2 = gameObject2.transform.Find("System/Speaker/Pivot");
				GameObject.Destroy(transform.GetComponent<MeshRenderer>());
				GameObject.Destroy(transform.GetComponent<MeshFilter>());
				GameObject.DestroyImmediate(transform.transform.Find("Pivot").gameObject);
				GameObject.DestroyImmediate(transform.transform.Find("Pivot").gameObject);
				transform.transform.Find("TunerPivot").gameObject.SetActive(false);
				transform.transform.Find("ButtonsRadio/RadioVolume").gameObject.SetActive(false);
				transform2.SetParent(transform.transform.parent.Find("Speaker"), false);
				PlayMakerFSM component = transform.transform.parent.Find("Speaker").GetComponent<PlayMakerFSM>();
				gameObject2.transform.Find("System/Speaker").GetComponent<Speaker>().hasSubwoofers = component.FsmVariables.FindFsmBool("LowPass").Value;
				ModConsole.Log("<color=purple>MRS : Finished Replacing Radio in Vehicle: " + transform.transform.root.name + "!</color>");
			}
		}
		private void ApplySatsumaStockRadio()
		{
			if (save == null)
			{
				save = new SaveClass();
			}
			GameObject satsRadio = Resources.FindObjectsOfTypeAll<GameObject>()
			.Where(x => x.name == "radio(Clone)")
			.First((GameObject x) => x.GetComponent<PlayMakerFSM>() != null);

			GameObject gameObject2 = GameObject.Instantiate<GameObject>(RadioPrefab);
			gameObject2.transform.SetParent(satsRadio.transform, false);
			gameObject2.transform.localEulerAngles = Vector3.zero;
			GameObject.Destroy(satsRadio.GetComponent<MeshRenderer>());
			GameObject.Destroy(satsRadio.GetComponent<MeshFilter>());
			GameObject.DestroyImmediate(satsRadio.transform.Find("Pivot").gameObject);
			GameObject.DestroyImmediate(satsRadio.transform.Find("Pivot").gameObject);
			satsRadio.transform.Find("TunerPivot").gameObject.SetActive(false);
			satsRadio.transform.Find("ButtonsRadio/RadioVolume").gameObject.SetActive(false);
			Transform transform = gameObject2.transform.Find("System/Speaker/Pivot");
			GameObject satsuma = GameObject.Find("SATSUMA(557kg, 248)");
			Transform transform2 = satsuma.transform.GetComponentsInChildren<Transform>(true).First((Transform x) => x.name == "CDPlayer");
			transform.SetParent(transform2.parent, false);
			gameObject2.transform.Find("System/Speaker").GetComponent<Speaker>().hasSubwoofers = false;
			satsuma.transform.Find("Electricity/PowerON").gameObject.AddComponent<SatsElectricityFix>().radioElec = gameObject2.GetComponent<RadioElectricity>();
			SatsRadioDisassembleFix satsRadioDisassembleFix = gameObject2.AddComponent<SatsRadioDisassembleFix>();
			satsRadioDisassembleFix.radioElec = gameObject2.GetComponent<RadioElectricity>();
			satsRadioDisassembleFix.sats = satsuma.transform;
		}
		private void ReplacePortableRadios()
		{
			GameObject gameObject = Resources.FindObjectsOfTypeAll<GameObject>().First((GameObject x) => x.name == "radio(itemx)");

			foreach (Transform transform in gameObject.transform.GetComponentsInChildren<Transform>(true))
			{
				if (transform.name != "mesh" && transform.name != "radio(itemx)")
					transform.gameObject.SetActive(false);
			}

			GameObject.Instantiate<GameObject>(boomboxPrefab).transform.SetParent(gameObject.transform, false);
			GameObject gameObject2 = 
				Resources.FindObjectsOfTypeAll<GameObject>()
			.Where(x => x.name == "radio(Clone)")
			.First((GameObject x) => x.GetComponent<PlayMakerFSM>() == null);

			foreach (Transform transform2 in gameObject2.transform.GetComponentsInChildren<Transform>(true))
			{
				if (transform2.name != "mesh" && transform2.name != "radio(Clone)")
					transform2.gameObject.SetActive(false);
			}

			GameObject.Instantiate<GameObject>(boomboxPrefab).transform.SetParent(gameObject2.transform, false);
		}

		private void CreateFolk()
		{
			string[] array = Directory.GetFiles(mainPath + "Radio\\");
			array = 
				array.Where(x => !x.EndsWith(".png"))
				.ToArray();

			List<AudioClip> preFillAudioClipList = 
				FsmVariables.GlobalVariables
				.FindFsmGameObject("SongDatabase").Value
				.GetComponentsInChildren<PlayMakerArrayListProxy>()
				.First((PlayMakerArrayListProxy x) => x.referenceName == "Ads")
				.preFillAudioClipList;

			RadioChannels.AddChannel(80.2f, "ToiveRadio", mainPath + "Radio\\", array, preFillAudioClipList.ToArray());
		}
		private void CreateChannel1()
		{
			PlayMakerArrayListProxy[] componentsInChildren = GameObject.Find("RADIO").transform.Find("Paikallisradio").GetComponentsInChildren<PlayMakerArrayListProxy>();
			List<AudioClip> preFillAudioClipList = componentsInChildren.First((PlayMakerArrayListProxy x) => x.referenceName == "Music").preFillAudioClipList;
			List<AudioClip> preFillAudioClipList2 = componentsInChildren.First((PlayMakerArrayListProxy x) => x.referenceName == "Programs").preFillAudioClipList;
			RadioChannels.AddChannel(91f, "Alivieskan", string.Empty, preFillAudioClipList.ToArray(), preFillAudioClipList2.ToArray());
		}
		private void CreateCustomChannels()
		{
			foreach (string text in Directory.GetDirectories(customRadioPath))
			{
				string[] array = text.Replace(customRadioPath + "\\", "").Split(new char[]
				{
					','
				});
				string name = array[0];
				float frequency = float.Parse(array[1]);
				IEnumerable<string> enumerable = from x in Directory.GetFiles(text)
				where !x.EndsWith(".txt")
				select x;
				string[] source = new string[0];
				if (File.Exists(text + "\\ads.txt"))
				{
					source = File.ReadAllLines(text + "\\ads.txt");
				}
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				foreach (string text2 in enumerable)
				{
					string fileName = Path.GetFileName(text2);
					if (!source.Contains(fileName))
					{
						list.Add(text2);
					}
					else
					{
						list2.Add(text2);
					}
				}
				RadioChannels.AddChannel(frequency, name, text + "\\", list.ToArray(), list2.ToArray());
			}
		}
	}
}
