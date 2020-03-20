using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.IO.IsolatedStorage;

public class Localizer
{
	public static ITranslationObject[] Translations;
	public static ITranslationObject[] SharedTranslations;

	public static void StartLocalizer()
	{
		//SceneManager.sceneLoaded += OnSceneLoaded;
		UpdateLabels();
	}

	public static void UpdateLabels()
	{
		LoadTranslations();
		if (SharedTranslations != null || Translations != null)
		{
			UILabel[] Labels = Resources.FindObjectsOfTypeAll<UILabel>();

			if (Labels.Length <= 0)
			{
				Debug.LogError("No UILabels found in the scene!");
			}
			else if (Translations == null || Translations.Length == 0)
			{
				Debug.LogError("The Translations Array is null or empty !");
			}
			InitializeTranslators(Labels);
		}
	}

	public static void LoadTranslations()
	{
		if (SharedTranslations == null)
		{
			try
			{
				SharedTranslations = LocalizerJSON.LoadFromJSON(Application.streamingAssetsPath + "/Yandere_Next/Mods/default/Localization/French/Shared/Localization.json");
			}
			catch
			{
				SharedTranslations = new ITranslationObject[0];
			}
		}
		try
		{
			Translations = LocalizerJSON.LoadFromJSON(Application.streamingAssetsPath + "/Yandere_Next/Mods/default/Localization/French/" + SceneManager.GetActiveScene().name + "/Localization.json").Union(SharedTranslations).ToArray();
		}
		catch (Exception ex)
		{
			Debug.Log("Couldn't load translation file " + ex.Message);
			Translations = SharedTranslations;
		}
	}

	static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		UpdateLabels();
	}

	static void InitializeTranslators(UILabel[] Labels)
	{
		foreach (UILabel label in Labels)
		{
			Translator ts = label.gameObject.GetComponent<Translator>();
			if (ts != null)
			{
				ts.Label = label;
			}
			else
			{
				ts = label.gameObject.AddComponent<Translator>();
				ts.Label = label;
			}
		}
	}
}
