namespace Chigen.Core.Services;

public static class CellSortHelper
{
    public static string SortKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return "ZZ";
        if (key.Length == 1 && key[0] >= '0' && key[0] <= '9')
            return $"0{key}";
        return $"1{key}";
    }
}
