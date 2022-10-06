using System.Text;

namespace Andreal.Core.Utils;

internal static class AbbreviationHelper
{
    internal static string GetAbbreviation(this string str)
    {
        var sb = new StringBuilder();
        sb.Append(str[0]);

        for (var index = 0; index < str.Length - 1; ++index)
        {
            if (str[index] == ' ') sb.Append(str[index + 1]);
        }

        return sb.ToString();
    }
}
