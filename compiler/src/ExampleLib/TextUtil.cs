using System.Globalization;
using System.Text;

namespace ExampleLib;

public static class TextUtil
{
    // Символы Unicode, которые мы принимаем как дефис.
    private static readonly Rune[] Hyphens = [new Rune('‐'), new Rune('-')];

    // Символы Unicode, которые мы принимаем как апостроф.
    private static readonly Rune[] Apostrophes = [new Rune('\''), new Rune('`')];

    // Состояния распознавателя слов.
    private enum WordState
    {
        NoWord,
        Letter,
        Hyphen,
        Apostrophe,
    }

    // Символы Unicode, которые мы принимаем как нижнее подчёркивание.
    private static readonly Rune[] Underscope = [new Rune('_'), new Rune('_')];

    // Состояния распознавателя слов.
    private enum CamelWordState
    {
        NoWord,
        Letter,
        Digit,
        UnderScope
    }

    /// <summary>
    ///  Распознаёт слова в тексте. Поддерживает Unicode, в том числе английский и русский языки.
    ///  Слово состоит из букв, может содержать дефис в середине и апостроф в середине либо в конце.
    /// </summary>
    /// <remarks>
    ///  Функция использует автомат-распознаватель с четырьмя состояниями:
    ///   1. NoWord — автомат находится вне слова;
    ///   2. Letter — автомат находится в буквенной части слова;
    ///   3. Hyphen — автомат обработал дефис;
    ///   4. Apostrophe — автомат обработал апостроф.
    ///
    ///  Переходы между состояниями:
    ///   - NoWord → Letter — при получении буквы;
    ///   - Letter → Hyphen — при получении дефиса;
    ///   - Letter → Apostrophe — при получении апострофа;
    ///   - Letter → NoWord — при получении любого символа, кроме буквы, дефиса или апострофа;
    ///   - Hyphen → Letter — при получении буквы;
    ///   - Hyphen → NoWord — при получении любого символа, кроме буквы;
    ///   - Apostrophe → Letter — при получении буквы;
    ///   - Apostrophe → NoWord — при получении любого символа, кроме буквы.
    ///
    ///  Разница между состояниями Hyphen и Apostrophe в том, что дефис не может стоять в конце слова.
    /// </remarks>
    public static List<string> ExtractWords(string text)
    {
        WordState state = WordState.NoWord;

        List<string> results = [];
        StringBuilder currentWord = new();
        foreach (Rune ch in text.EnumerateRunes())
        {
            switch (state)
            {
                case WordState.NoWord:
                    if (Rune.IsLetter(ch))
                    {
                        PushCurrentWord();
                        currentWord.Append(ch);
                        state = WordState.Letter;
                    }

                    break;

                case WordState.Letter:
                    if (Rune.IsLetter(ch))
                    {
                        currentWord.Append(ch);
                    }
                    else if (Hyphens.Contains(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Hyphen;
                    }
                    else if (Apostrophes.Contains(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Apostrophe;
                    }
                    else
                    {
                        state = WordState.NoWord;
                    }

                    break;

                case WordState.Hyphen:
                    if (Rune.IsLetter(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Letter;
                    }
                    else
                    {
                        // Убираем дефис, которого не должно быть в конце слова.
                        currentWord.Remove(currentWord.Length - 1, 1);
                        state = WordState.NoWord;
                    }

                    break;

                case WordState.Apostrophe:
                    if (Rune.IsLetter(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Letter;
                    }
                    else
                    {
                        state = WordState.NoWord;
                    }

                    break;
            }
        }

        PushCurrentWord();

        return results;

        void PushCurrentWord()
        {
            if (currentWord.Length > 0)
            {
                results.Add(currentWord.ToString());
                currentWord.Clear();
            }
        }
    }


    /// <summary>
    ///  Форматирует идентификатор в lower camel case нотацию.
    ///  Идентификатором считается любой набор английского алфавита, нижнего подчёркивания(_) и цифр,
    ///  который начинается исключительно с буквы.
    /// </summary>
    /// <remarks>
    ///  Функция использует автомат-распознаватель с четырьмя состояниями:
    ///   1. NoWord — автомат находится вне идентификатора;
    ///   2. Letter — автомат находится в буквенной части идентификатора;
    ///   3. Digit — автомат находится в части идентификатора с цифрой;
    ///   4. UnderScope — автомат обработал нижнее подчёркивание.
    ///
    ///  Переходы между состояниями:
    ///   - NoWord → Letter — при получении буквы(A..Z + a..z);
    ///   - Letter → Digit — при получении цифры(0...9);
    ///   - Letter → UnderScope — при получении нижнего подчёркивания(_);
    ///   - Letter → NoWord — при получении любого символа, кроме буквы, цифры или нижнего подчёркивания;
    ///   - Digit → Letter — при получении буквы;
    ///   - Digit → NoWord — при получении любого символа, кроме буквы или цифры;
    ///   - UnderScope → Letter — при получении буквы;
    ///   - UnderScope → NoWord — при получении любого символа, кроме буквы или нижнего подчёркивания.
    /// </remarks>
    public static string FormatLowerCamelCase(string identifier)
    {
        CamelWordState state = CamelWordState.NoWord;
        string formatIdentifier = "";
        foreach (Rune ch in identifier.EnumerateRunes())
        {
            switch (state)
            {
                case CamelWordState.NoWord: // _ and 0..9
                    if (Rune.IsLetter(ch))
                    {
                        if (formatIdentifier == "")
                        {
                            formatIdentifier += Rune.ToLowerInvariant(ch).ToString();
                        }
                        else
                        {
                            formatIdentifier += Rune.ToUpperInvariant(ch).ToString();
                        }
                        state = CamelWordState.Letter;
                    } else
                    {
                        throw new ArgumentException($"Некорректный идентификатор: {identifier}.");
                    }
                    break;
                case CamelWordState.Letter:
                    if (Rune.IsLetter(ch))
                    {
                        if (Rune.IsUpper(ch))
                        {
                            formatIdentifier += ch.ToString();
                        }
                        else
                        {
                            formatIdentifier += Rune.ToLowerInvariant(ch).ToString();
                        }
                    }
                    else if (Rune.IsDigit(ch))
                    {
                        formatIdentifier += ch.ToString();
                        state = CamelWordState.Digit;
                    }
                    else if (Underscope.Contains(ch))
                    {
                        state = CamelWordState.UnderScope;
                    } else
                    {
                        state = CamelWordState.NoWord;
                    }
                    break;
                case CamelWordState.Digit:
                    if (Rune.IsLetter(ch))
                    {
                        formatIdentifier += Rune.ToUpperInvariant(ch).ToString();
                        state = CamelWordState.Letter;
                    }
                    else if (Rune.IsDigit(ch))
                    {
                        formatIdentifier += ch.ToString();
                    }
                    else if (Underscope.Contains(ch))
                    {
                        state = CamelWordState.UnderScope;
                    } else
                    {
                        state = CamelWordState.NoWord;
                    }
                    break;
                case CamelWordState.UnderScope:
                    if (Rune.IsLetter(ch))
                    {
                        formatIdentifier += Rune.ToUpperInvariant(ch).ToString();
                        state = CamelWordState.Letter;
                    } else if (Rune.IsDigit(ch))
                    {
                        formatIdentifier += ch.ToString();
                        state = CamelWordState.Digit;
                    } else if (Underscope.Contains(ch))
                    {
                        break; // save underscope state
                    } else
                    {
                        state = CamelWordState.NoWord;
                    }
                    break;
            }
        }
        return formatIdentifier;
    }
}