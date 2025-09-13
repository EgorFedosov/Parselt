namespace Backend.Infrastructure.Utils;

public static class CsvLineParser
{
    public static  string[] ParseCsvLine(string line)
    {
        var values = new List<string>();
        var currentValue = new System.Text.StringBuilder();
        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char currentChar = line[i];

            if (currentChar == '"')
            {
                if (insideQuotes)
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentValue.Append('"');
                        i++;
                    }
                    else
                    {
                        insideQuotes = false;
                    }
                }
                else
                {
                    insideQuotes = true;
                }
            }
            else if (currentChar == ',' && !insideQuotes)
            {
                values.Add(currentValue.ToString().Trim());
                currentValue.Clear();
            }
            else
            {
                currentValue.Append(currentChar);
            }
        }

        values.Add(currentValue.ToString().Trim());

        return values.ToArray();
    }

    public static string[] ParseCsvLine(string line, string delimiter)
    {
        var values = new List<string>();
        var currentValue = new System.Text.StringBuilder();
        bool insideQuotes = false;
        int delimLength = delimiter.Length;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '"')
            {
                if (insideQuotes)
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentValue.Append('"');
                        i++;
                    }
                    else
                    {
                        insideQuotes = false;
                    }
                }
                else
                {
                    insideQuotes = true;
                }
            }
            else if (!insideQuotes && i + delimLength - 1 < line.Length && line.Substring(i, delimLength) == delimiter)
            {
                values.Add(currentValue.ToString().Trim());
                currentValue.Clear();
                i += delimLength - 1; 
            }
            else
            {
                currentValue.Append(line[i]);
            }
        }

        values.Add(currentValue.ToString().Trim());
        return values.ToArray();
    }

}