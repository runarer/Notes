using System.Collections;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace NotesWeb.Commons;

internal sealed class RedactionProcessor : BaseProcessor<LogRecord>
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
                if (entryVal != null && entryVal.Contains("token"))
                {
                    return new KeyValuePair<string, object?>(item.Key, "***REDACTED***");
                }
                if (entryVal != null && entryVal.Contains("password"))
                {
                    return new KeyValuePair<string, object?>(item.Key, "***REDACTED***");
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
}

