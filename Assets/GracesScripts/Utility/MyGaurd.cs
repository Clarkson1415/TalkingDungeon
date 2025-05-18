using System;
using System.Diagnostics.CodeAnalysis;
#nullable enable

public class MyGuard
{
    public static void IsNotNull<T>([NotNull] T? value)
    {
        if(value is not null)
        {
            return;
        }

        throw new ArgumentException();
    }

    public static void IsNotNull<T>([NotNull] T? value, string failureMessage)
    {
        if (value is not null)
        {
            return;
        }

        throw new ArgumentException($"MyGuard IsNotNull exception for {value} msg: {failureMessage}");
    }
}