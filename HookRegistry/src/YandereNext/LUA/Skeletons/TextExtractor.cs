
using System.Text;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Reflection;
using YandereNext;
using YandereSimulator.Yancord;
using UnityEngine.SceneManagement;
using System;

class TextExtractor : MonoBehaviour
{
	public static string EncodeJsString(string s)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("\"");
		foreach (char c in s)
		{
			switch (c)
			{
				//case '\'':
				//sb.Append("\\\'");
				//break;
				case '\"':
					sb.Append("\\\"");
					break;
				case '\\':
					sb.Append("\\\\");
					break;
				case '\b':
					sb.Append("\\b");
					break;
				case '\f':
					sb.Append("\\f");
					break;
				case '\n':
					sb.Append("\\n");
					break;
				case '\r':
					sb.Append("\\r");
					break;
				case '\t':
					sb.Append("\\t");
					break;
				default:
					int i = (int)c;
					if (i < 32 || i > 127)
					{
						sb.AppendFormat("\\u{0:X04}", i);
					}
					else
					{
						sb.Append(c);
					}
					break;
			}
		}
		sb.Append("\"");

		return sb.ToString();
	}
	public static string FixString(string Match, string Translation)
	{
		return EncodeJsString(Match) + " : " + EncodeJsString(Translation);
	}


	public static void WriteFile(string json)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(json));
		if (File.Exists(json))
		{
			File.Delete(json);
		}
		using (var file = File.CreateText(json))
		{
			file.WriteLine("[{");
			int i = 0;
			foreach (var line in TranslationObjects)
			{
				string text = FixString(line.GetPattern(), line.GetTranslationPattern());
				if (i < TranslationObjects.Count - 1)
				{
					i++;
					text += ",";
				}
				Debug.Log(text);
				file.WriteLine(text);
			}
			file.WriteLine("}]");
		}
	}
	public static void WriteSub(string text)
	{
		foreach (ITranslationObject TranslationObject in TranslationObjects)
		{
			if ((TranslationObject.GetPattern() == text) || TranslationObject.Matches(text))
			{
				return;
			}
		}
		TranslationObjects.Add(new TranslationObject(text, text));
	}

	public static void ExtractingStringArrays<T>(T instance)
	{
		if (instance != null)
		{
			FieldInfo[] Fields = typeof(T).GetFields();
			foreach (var Field in Fields)
			{
				if (Field.FieldType == typeof(string[]))
				{
					string[] subs = (string[])Field.GetValue(instance);
					foreach (string s in subs)
					{
						if (!string.IsNullOrEmpty(s))
						{
							WriteSub(s);
						}
					}
				}
			}
		}
	}

	public static void ExtractingStandardSubtitles()
	{
		SubtitleScript subtitles = UnityEngine.Object.FindObjectOfType<SubtitleScript>();
		if (subtitles != null)
		{
			ExtractingStringArrays(subtitles);
		}
	}

	public static void ExtractingLabels()
	{
		UILabel[] labels = UnityEngine.Object.FindObjectsOfType<UILabel>();

		foreach (var label in labels)
		{
			if (!string.IsNullOrEmpty(label.text))
			{
				WriteSub(label.text);
			}
		}
	}

	public static void ExtractingLaptopDiscussion()
	{
		LaptopScript laptop = UnityEngine.Object.FindObjectOfType<LaptopScript>();
		if (laptop != null)
		{
			foreach (string sub in laptop.Subs)
			{
				WriteSub(sub);
			}
		}
	}

	public static void ExtractingFunGirlDiscussion()
	{
		FunScript fun = UnityEngine.Object.FindObjectOfType<FunScript>();
		WriteSub(fun.Typewriter.mFullText);
	}

	public static void ExtractingYanCordDiscussions()
	{
		YancordManager manager = UnityEngine.Object.FindObjectOfType<YancordManager>();
		if (manager != null)
		{
			foreach (NewTextMessage message in manager.Dialogue)
			{
				WriteSub(message.Message);
				WriteSub(message.OptionF);
				WriteSub(message.OptionQ);
				WriteSub(message.OptionR);
				WriteSub(message.ReactionF);
				WriteSub(message.ReactionQ);
				WriteSub(message.ReactionR);
			}

		}
	}
	public static void ExtractShopText()
	{
		Debug.Log("Starting Shop Text extraction !");
		
		StreetShopScript[] Shops = FindObjectsOfType<StreetShopScript>();
		foreach (StreetShopScript Shop in Shops)
		{
			ExtractingStringArrays(Shop);
			Debug.Log("Extracting Shop Interface " + Shop.name);
			

		}
	}

	public static void ExtractAllStringsFromClass<T>()
	{
		try
		{
			UnityEngine.Object[] Objects = UnityEngine.Object.FindObjectsOfType(typeof(T));
			foreach (var Object in Objects)
			{
				ExtractingStringArrays(Object);
			}
		}
		catch
		{
			Debug.Log("an error occurred");
		}

	}

	public static void ExtractingTapeDiscussions()
	{
		TapePlayerMenuScript tapemenu = UnityEngine.Object.FindObjectOfType<TapePlayerMenuScript>();

		ExtractingStringArrays(tapemenu);
	}

	public static void ExtractingNoteWindowStrings()
	{
		NoteWindowScript notes = UnityEngine.Object.FindObjectOfType<NoteWindowScript>();
		ExtractingStringArrays(notes);
	}

	

	public static List<ITranslationObject> TranslationObjects = new List<ITranslationObject>();
	public IEnumerator Extract(string SceneName)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		TranslationObjects.Clear();
		string json = YandereNextManager.ModDir + "\\Localization\\French\\" + SceneName + "\\Localization.json";
		Directory.CreateDirectory(Path.GetDirectoryName(json));
		if (!File.Exists(json))
		{
			using (TextWriter writer = File.CreateText(json))
			{
				writer.WriteLine("[{");
				writer.WriteLine("");
				writer.WriteLine("}]");
			}
		}
		ITranslationObject[] JSONObjects = LocalizerJSON.LoadFromJSON(json);

		foreach (ITranslationObject JSONObject in JSONObjects)
		{
			TranslationObjects.Add(JSONObject);
		}

		ExtractingLabels();
		ExtractingStandardSubtitles();
		switch (SceneName)
		{
			case "SchoolScene":
				ExtractingLaptopDiscussion();
				ExtractingNoteWindowStrings();
				break;
			case "YanCordScene":
				ExtractingYanCordDiscussions();
				break;
			case "FunScene":
			case "MoreFunScene":
			case "VeryFunScene":
				ExtractingFunGirlDiscussion();
				break;
			case "StreetScene":
				ExtractShopText();
				break;
		}

		WriteFile(json);
	}
	 void OnSceneLoad(Scene scene, LoadSceneMode mode)
	{
		StartCoroutine(Extract(scene.name));
	}

	public static void ExtractingText()
	{
		TextExtractor TE = new GameObject("TextExtractor").AddComponent<TextExtractor>();
		SceneManager.sceneLoaded += TE.OnSceneLoad;
		TE.StartCoroutine(TE.Extract(SceneManager.GetActiveScene().name));
		DontDestroyOnLoad(TE);
	}


}


