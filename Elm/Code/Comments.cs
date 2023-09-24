namespace ElmX.Elm.Code
{
    public class Comments
    {

        static public string Remove(string elmString)
        {
            elmString = RemoveSingleLineComments(elmString);

            elmString = RemoveMultilineComments(elmString);

            return elmString;
        }

        static private string RemoveSingleLineComments(string elmString)
        {
            string[] lines = elmString.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("--"))
                {
                    lines[i] = lines[i].Replace("--", "");
                }
                else if (lines[i].StartsWith("{-"))
                {
                    lines[i] = lines[i].Replace("{-", "").Replace("-}", "");
                }
                else if (lines[i].Contains("--"))
                {
                    lines[i] = RemoveSingleLineComment(lines[i]);
                }
            }

            elmString = string.Join("\n", lines);

            return elmString;
        }

        static private string RemoveSingleLineComment(string line)
        {
            int index = line.IndexOf("--");

            line = line.Remove(index);

            return line;
        }

        static private string RemoveMultilineComments(string elmString)
        {
            string[] lines = elmString.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("{-") && lines[i].Contains("-}"))
                {
                    lines[i] = RemoveMultilineComment(lines[i]);

                }
                else if (lines[i].Contains("{-"))
                {
                    lines[i] = RemoveMultilineComment(lines[i]);

                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (lines[j].Contains("-}"))
                        {
                            lines[j] = RemoveMultilineComment(lines[j]);

                            i = j;

                            break;
                        }
                        else
                        {
                            lines[j] = "";
                        }
                    }
                }
            }

            elmString = string.Join("\n", lines);

            return elmString;
        }

        static private string RemoveMultilineComment(string line)
        {
            int startIndex = line.IndexOf("{-");
            int endIndex = line.IndexOf("-}");
            int lastIndex = line.LastIndexOf("{-");

            if (lastIndex > startIndex)
            {
                line = line.Remove(lastIndex, line.Length - lastIndex);
            }

            if (startIndex > -1 && endIndex > -1)
            {
                line = line.Remove(startIndex, endIndex - startIndex);
                line = line.Replace("{-", "");
                line = line.Replace("-}", "");
            }
            else if (startIndex > -1)
            {
                line = line.Remove(startIndex);
                line = line.Replace("{-", "");
            }
            else if (endIndex > -1)
            {
                line = line.Remove(endIndex);
                line = line.Replace("-}", "");
            }

            return line.Trim();
        }
    }
}