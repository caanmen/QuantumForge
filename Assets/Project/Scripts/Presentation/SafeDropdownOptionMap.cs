using System;
using System.Collections.Generic;
using TMPro;


public sealed class SafeDropdownOptionMap<T>
{
    private readonly List<T> _visibleOptionIds = new List<T>();
    private readonly IEqualityComparer<T> _comparer;

    public SafeDropdownOptionMap() : this(EqualityComparer<T>.Default) { }

    public SafeDropdownOptionMap(IEqualityComparer<T> comparer)
    {
        _comparer = comparer ?? EqualityComparer<T>.Default;
    }

    public IReadOnlyList<T> VisibleOptionIds => _visibleOptionIds;

    public int Rebuild(
        TMP_Dropdown dropdown,
        IList<T> optionIds,
        Func<T, string> labelForId,
        T preferredId)
    {
        _visibleOptionIds.Clear();
        var labels = new List<string>();
        if (optionIds != null)
        {
            for (int i = 0; i < optionIds.Count; i++)
            {
                T id = optionIds[i];
                if (IndexOf(id) >= 0) continue;
                _visibleOptionIds.Add(id);
                labels.Add(labelForId == null ? Convert.ToString(id) : labelForId(id));
            }
        }

        int selectedIndex = IndexOf(preferredId);
        if (selectedIndex < 0) selectedIndex = _visibleOptionIds.Count > 0 ? 0 : -1;
        if (dropdown != null)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(labels);
            if (selectedIndex >= 0) dropdown.value = selectedIndex;
            dropdown.RefreshShownValue();
        }
        return selectedIndex;
    }

    public bool TryResolve(int visibleIndex, out T id)
    {
        if (visibleIndex >= 0 && visibleIndex < _visibleOptionIds.Count)
        {
            id = _visibleOptionIds[visibleIndex];
            return true;
        }
        id = default(T);
        return false;
    }

    public T ResolveOrDefault(int visibleIndex, T fallback)
    {
        return TryResolve(visibleIndex, out T id) ? id : fallback;
    }

    public int IndexOf(T id)
    {
        for (int i = 0; i < _visibleOptionIds.Count; i++)
            if (_comparer.Equals(_visibleOptionIds[i], id)) return i;
        return -1;
    }
}
