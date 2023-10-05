namespace ElmX.Core
{
    public class String
    {

        static public string ExtractInner((char first, char last) enclosing, string content)
        {
            return ExtractInner(enclosing, content, (0, 0), (0, -1));
        }

        static private string ExtractInner((char first, char last) enclosing, string content, (int start, int end) counter, (int current, int start) indexer)
        {
            char c = content[indexer.current];

            if (c == enclosing.first)
            {
                if (indexer.start == -1)
                {
                    indexer.start = indexer.current + 1;
                }
                counter.start++;
            }
            else if (c == enclosing.last)
            {
                counter.end++;
            }

            if (counter.start == counter.end)
            {
                return content[indexer.start..indexer.current];
            }

            return ExtractInner(enclosing, content, counter, (indexer.current + 1, indexer.start));
        }
    }
}
