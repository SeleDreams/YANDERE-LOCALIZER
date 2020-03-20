public interface ITranslationObject
{
	string Translate(string text);
	string GetPattern();
	string GetTranslationPattern();
	bool Matches(string text);
	bool isLitteral();
}

