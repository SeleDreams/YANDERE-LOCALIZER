using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
public class TranslationObject : ITranslationObject
{
	class Variable
	{
		public string Name;
		public string Value;
	}

	public readonly string RecognitionPattern;
	public readonly string Translation;

	readonly Regex GameTextRegex;
	readonly Regex VariablesRecognitionRegex = new Regex("(\\${(.*?)(?=})})(.*?(?=(\\${)|($)))");

	List<Variable> Variables = new List<Variable>();

	
	public TranslationObject(string PatternArgument, string TranslationArgument)
	{
		RecognitionPattern = PatternArgument;
		Translation = TranslationArgument;

		if (!isLitteral())
		{
			string PreviousText = null;

			// Get all the variables matches of the translation
			MatchCollection Matches = VariablesRecognitionRegex.Matches(PatternArgument);

			// Make sure to escape the string to avoid conflicting with regex
			PatternArgument = Regex.Escape(PatternArgument);
			
			// Each match will create an item 
			foreach (Match match in Matches)
			{
				string varName = match.Groups[1].Value;
				string NextPart = Regex.Escape(match.Groups[3].Value);
				int VariableIndex = RecognitionPattern.IndexOf(varName);
				if (VariableIndex == 0)
				{
					PreviousText = "^";
				}
				if (VariableIndex + varName.Length == RecognitionPattern.Length)
				{
					NextPart = "$";
					Debug.Log("NOTHING AFTER VARIABLE,REPLACED BY $ TO AVOID ERRORS");
				}

				// The formula is basically "Find everything between the previous text and the next text"
				string RegexFormula = PreviousText + "(" + ".*?(?=" + NextPart + "))";

				PreviousText = NextPart;
				var item = new Variable() { Name = varName };
				Variables.Add(item);
				PatternArgument = PatternArgument.Replace(Regex.Escape(match.Value), RegexFormula);
			}
			GameTextRegex = new Regex(PatternArgument);
		}
	}


	public string Translate(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			if (Matches(text))
			{
				string TranslatedText = Translation;
				Match TextMatches = GameTextRegex.Match(text);
				for (int i = 1; i < TextMatches.Groups.Count; i++)
				{
					Variables[i - 1].Value = TextMatches.Groups[i].Value;

				}
				foreach (Variable item in Variables)
				{
					if (TranslatedText.Contains(item.Name))
					{
						TranslatedText = TranslatedText.Replace(item.Name, item.Value);
						Debug.Log(item.Name + " is " + item.Value);
					}
				}
				return TranslatedText;
			}
			else if (text.Length == RecognitionPattern.Length && text == RecognitionPattern)
			{
				return Translation;
			}
			else
			{
				return null;
			}
		}
		else
		{
			return null;
		}
	}

	public string GetPattern()
	{
		return RecognitionPattern;
	}

	public string GetTranslationPattern()
	{
		return Translation;
	}

	public bool Matches(string text)
	{
		return GameTextRegex != null && GameTextRegex.IsMatch(text);
	}

	public bool isLitteral()
	{
		return !VariablesRecognitionRegex.IsMatch(RecognitionPattern);
	}
}

