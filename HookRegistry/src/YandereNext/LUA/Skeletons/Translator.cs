using UnityEngine;

public class Translator : MonoBehaviour
{
	public UILabel Label;
	public bool Updateable = true;

	private string OriginalText = string.Empty;
	private string TranslatedText = string.Empty;

	public static string Translate(string text, ITranslationObject Translation)
	{
		return Translation.Translate(text);
	}

	public void Start()
	{
		UpdateTranslation();
	}

	public void LateUpdate()
	{
		if (Updateable)
		{
			UpdateTranslation();
		}
	}

	/// <summary>
	/// ALWAYS updates the translation, this is costly so avoid to run it every frame when the text isn't updated.
	/// </summary>
	public void UpdateLabel()
	{
		if (Localizer.Translations != null && Localizer.Translations.Length > 0)
		{
			foreach (var Translation in Localizer.Translations)
			{
				string TranslationResult = Translate(Label.text, Translation);
				if (TranslationResult != null)
				{
					Debug.Log(TranslationResult);
					OriginalText = Label.text;
					TranslatedText = TranslationResult;
					Label.text = TranslationResult;
				}
			}
		}
		else
		{
			Debug.LogError("The Translation of the Localizer has not been initialized or is empty, the translator will stop updating !");
		}
	}

	/// <summary>
	/// Updates the translation if the text is visible and has changed.
	/// </summary>
	public void UpdateTranslation()
	{
		bool TextVisible = Label.isVisible && !string.IsNullOrEmpty(Label.text);
		bool TextChanged = Label.text.Length != TranslatedText.Length && Label.text != TranslatedText;

		if (TextVisible)
		{
			bool WasRestoredToOriginalText = Label.text.Length == OriginalText.Length && Label.text == OriginalText;
			if (TextChanged &&  !string.IsNullOrEmpty(TranslatedText) && WasRestoredToOriginalText)
			{
				Label.text = TranslatedText;
			}
			else
			{
				UpdateLabel();
			}
		}
	}
	
	
}

