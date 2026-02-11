using System.Collections.Generic;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers;

internal sealed class EmptyParams : IReadOnlyDictionary<string, object?>
{
    public static readonly EmptyParams Instance = new();

    public int Count => 0;
    public IEnumerable<string> Keys { get { yield break; } }
    public IEnumerable<object?> Values { get { yield break; } }
    public object? this[string key] => throw new KeyNotFoundException();
    public bool ContainsKey(string key) => false;
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() { yield break; }
    public bool TryGetValue(string key, out object? value) { value = null; return false; }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}