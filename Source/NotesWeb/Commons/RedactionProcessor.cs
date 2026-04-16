using System.Collections;
using System.Text.RegularExpressions;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace NotesWeb.Commons;

internal sealed partial class RedactionProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord logRecord)
    {
        if (logRecord.Attributes != null)
        {
            logRecord.Attributes = new RedactionEnumerator(logRecord.Attributes);
        }
    }

    internal sealed class RedactionEnumerator(IReadOnlyList<KeyValuePair<string, object?>> state) : IReadOnlyList<KeyValuePair<string, object?>>
    {
        private readonly IReadOnlyList<KeyValuePair<string, object?>> state = state;

        public int Count => state.Count;

        public KeyValuePair<string, object?> this[int index]
        {
            get
            {
                var item = state[index];
                var entryVal = item.Value?.ToString();

                if (entryVal != null)
                {
                    // Redact passwords
                    var redactedValue = PasswordRegex.Replace(entryVal, "\"password\":\"***PASSWORD-REDACTED***\"");

                    // Redact token
                    redactedValue = TokenRegex.Replace(redactedValue, "\"token\":\"***TOKEN-REDACTED***\"");

                    if (redactedValue != entryVal)
                    {
                        return new KeyValuePair<string, object?>(item.Key, redactedValue);
                    }
                }

                return item;

            }
        }

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    [GeneratedRegex(@"""password""\s*:\s*"".*""", RegexOptions.IgnoreCase)]
    private static partial Regex PasswordRegex { get; }

    [GeneratedRegex(@"""token""\s*:\s*"".*""", RegexOptions.IgnoreCase)]
    private static partial Regex TokenRegex { get; }
}

