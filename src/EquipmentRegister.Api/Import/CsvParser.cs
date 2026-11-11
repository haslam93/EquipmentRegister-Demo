using System.Text;

namespace EquipmentRegister.Api.Import;

/// <summary>One line of a CSV file. <see cref="LineNumber"/> is 1-based and counts the header line.</summary>
public sealed record CsvLine(int LineNumber, IReadOnlyList<string> Fields);

public static class CsvParser
{
    /// <summary>
    /// Parses RFC 4180 style CSV: comma separated, double-quoted fields may contain commas,
    /// line breaks and escaped quotes (""). Blank lines are skipped but still counted.
    /// </summary>
    public static IReadOnlyList<CsvLine> Parse(string text)
    {
        var lines = new List<CsvLine>();
        var fields = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;
        var lineNumber = 1;
        var lineStart = 1;

        void EndField()
        {
            fields.Add(field.ToString());
            field.Clear();
        }

        void EndLine()
        {
            EndField();
            if (fields.Count > 1 || fields[0].Trim().Length > 0)
            {
                lines.Add(new CsvLine(lineStart, fields.ToArray()));
            }

            fields.Clear();
        }

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];

            if (inQuotes)
            {
                if (c == '"' && i + 1 < text.Length && text[i + 1] == '"')
                {
                    field.Append('"');
                    i++;
                }
                else if (c == '"')
                {
                    inQuotes = false;
                }
                else
                {
                    if (c == '\n')
                    {
                        lineNumber++;
                    }

                    field.Append(c);
                }

                continue;
            }

            switch (c)
            {
                case '"':
                    inQuotes = true;
                    break;
                case ',':
                    EndField();
                    break;
                case '\r':
                    break;
                case '\n':
                    EndLine();
                    lineNumber++;
                    lineStart = lineNumber;
                    break;
                default:
                    field.Append(c);
                    break;
            }
        }

        if (field.Length > 0 || fields.Count > 0)
        {
            EndLine();
        }

        return lines;
    }
}
