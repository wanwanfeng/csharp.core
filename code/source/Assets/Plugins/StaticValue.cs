
public static class StaticValue
{
    public static long ToLong(this string input)
    {
        long val = 0;
        return long.TryParse(input, out val) ? val : 0;
    }

    public static int ToInt(this string input)
    {
        int val = 0;
        return int.TryParse(input, out val) ? val : 0;
    }
}