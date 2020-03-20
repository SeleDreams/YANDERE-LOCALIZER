using System.Collections.Generic;

public class LocalizerJSON : JsonData
{
	const string RegexDetectionString = "{REGEX}=";

	public static ITranslationObject[] LoadFromJSON(string path)
	{
		var TranslationObjects = new List<ITranslationObject>();

		Dictionary<string,object>[] Dictionaries = Deserialize(path);

		foreach (var Dictionary in Dictionaries)
		{
			foreach (var Element in Dictionary)
			{
				if (Element.Key != "LOCALIZATION_COMMENT" || Element.Key != "LOCALIZATION_AUTHORS" || Element.Key != "LOCALIZATION_VERSION")
				{
					if (Element.Key.StartsWith(RegexDetectionString))
					{
						string regexString = Element.Key.Remove(0, RegexDetectionString.Length);
						var RegexObject = new RegexObject(regexString, Element.Value.ToString());
						TranslationObjects.Add(RegexObject);
					}
					else
					{
						var TranslationObject = new TranslationObject(Element.Key, Element.Value.ToString());
						TranslationObjects.Add(TranslationObject);
					}
				}
			}
		}
		return TranslationObjects.ToArray();
	}

}
