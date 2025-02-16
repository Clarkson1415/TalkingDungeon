using System;

public class MyGuard
{
    public static void IsNotNull<T>(T value)
    {
        if(value is not null)
        {
            return;
        }

        throw new ArgumentNullException(nameof(value));
    }
}