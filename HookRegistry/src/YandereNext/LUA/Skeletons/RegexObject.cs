using System.Collections.Generic;
using System.Text.RegularExpressions;

public class RegexObject : ITranslationObject
{
	public RegexObject(string regex, string translation)
	{
		Regex = new Regex(regex);
		Translation = translation;
	}

	public string Translate(string text)
	{
		if (Matches(text))
		{
			MatchCollection TranslationVariablesCollection = TranslationVariablesRegex.Matches(Translation);
			Match RegexMatch = Regex.Match(text);
			MatchCollection RegexMatches = Regex.Matches(text);

			var Variables = new Dictionary<int, string>();

			// Creates a dictionnary with all the variables of the translation ordered by id.
			foreach (Match Match in TranslationVariablesCollection)
			{
				Variables.Add(int.Parse(Match.Groups[2].Value), Match.Groups[1].Value);
			}
			// Replaces all the variables by their value in the translation
			string NewTranslation = Translation;
			if (RegexMatch.Groups.Count > 1)
			{
				for (int VariableID = 0; VariableID < RegexMatch.Groups.Count; VariableID++)
				{
					if (Variables.ContainsKey(VariableID))
					{
						NewTranslation = NewTranslation.Replace(Variables[VariableID], RegexMatch.Groups[VariableID].Value);
					}
				}
			}
			else
			{
				int id = 1;
				foreach (Match match in RegexMatches)
				{
					if (Variables.ContainsKey(id))
					{
						NewTranslation = NewTranslation.Replace(Variables[id], match.Value);
					}
					id++;
				}
			}
			return NewTranslation;
		}
		else
		{
			return null;
		}
	}

	public string GetPattern()
	{
		return Regex.ToString();
	}

	public string GetTranslationPattern()
	{
		return Translation;
	}

	public bool Matches(string text)
	{
		return Regex.IsMatch(text);
	}


	public bool isLitteral()
	{
		return false;
	}

	public Regex TranslationVariablesRegex = new Regex("(\\${MATCH(\\d)})");
	public Regex Regex;
	public string Translation;
}
