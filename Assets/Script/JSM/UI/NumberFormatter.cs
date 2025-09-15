using System;
public static class NumberFormatter
{
    public static string FormatNumber(double number)
    {
        if (number >= 1_000_000_000_000)
            return Math.Floor(number / 1_000_000_000_000d * 10) / 10 + "T";
        else if (number >= 1_000_000_000)
            return Math.Floor(number / 1_000_000_000d * 10) / 10 + "B";
        else if (number >= 1_000_000)
            return Math.Floor(number / 1_000_000d * 10) / 10 + "M";
        else if (number >= 1_000)
            return Math.Floor(number / 1_000d * 10) / 10 + "k";
        else
            return number.ToString("0");
    }
}
