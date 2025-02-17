using System;
using System.Diagnostics.CodeAnalysis;

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
}