using System.Runtime.CompilerServices;

namespace Veldrid; // Put this into the Veldrid namespace to maintain backwards compatibility.

/// <summary>
/// Helper methods for combining hash codes.
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Combine two hash codes into a single hash code.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Combine(int value1, int value2)
    {
        uint rol5 = ((uint)value1 << 5) | ((uint)value1 >> 27);
        return ((int)rol5 + value1) ^ value2;
    }

    /// <summary>Combine multiple hash codes into a single hash code.</summary>
    public static int Combine(int value1, int value2, int value3) => Combine(value1, Combine(value2, value3));
    /// <summary>Combine multiple hash codes into a single hash code.</summary>
    public static int Combine(int value1, int value2, int value3, int value4) => Combine(value1, Combine(value2, Combine(value3, value4)));
    /// <summary>Combine multiple hash codes into a single hash code.</summary>
    public static int Combine(int value1, int value2, int value3, int value4, int value5) => Combine(value1, Combine(value2, Combine(value3, Combine(value4, value5))));

    /// <summary>Combine multiple hash codes into a single hash code.</summary>
    public static int Combine(
        int value1,
        int value2,
        int value3,
        int value4,
        int value5,
        int value6
    ) =>
        Combine(
            value1,
            Combine(value2, Combine(value3, Combine(value4, Combine(value5, value6))))
        );

    /// <summary>Combine multiple hash codes into a single hash code.</summary>
    public static int Combine(
        int value1,
        int value2,
        int value3,
        int value4,
        int value5,
        int value6,
        int value7
    ) =>
        Combine(
            value1,
            Combine(
                value2,
                Combine(value3, Combine(value4, Combine(value5, Combine(value6, value7))))
            )
        );

    /// <summary>Combine multiple hash codes into a single hash code.</summary>
    public static int Combine(
        int value1,
        int value2,
        int value3,
        int value4,
        int value5,
        int value6,
        int value7,
        int value8
    ) =>
        Combine(
            value1,
            Combine(
                value2,
                Combine(
                    value3,
                    Combine(value4, Combine(value5, Combine(value6, Combine(value7, value8))))
                )
            )
        );

    /// <summary>Combine multiple hash codes into a single hash code.</summary>
    public static int Combine(
        int value1,
        int value2,
        int value3,
        int value4,
        int value5,
        int value6,
        int value7,
        int value8,
        int value9
    ) =>
        Combine(
            value1,
            Combine(
                value2,
                Combine(
                    value3,
                    Combine(
                        value4,
                        Combine(value5, Combine(value6, Combine(value7, Combine(value8, value9))))
                    )
                )
            )
        );

    /// <summary>Combine multiple hash codes into a single hash code.</summary>
    public static int Combine(
        int value1,
        int value2,
        int value3,
        int value4,
        int value5,
        int value6,
        int value7,
        int value8,
        int value9,
        int value10
    ) =>
        Combine(
            value1,
            Combine(
                value2,
                Combine(
                    value3,
                    Combine(
                        value4,
                        Combine(
                            value5,
                            Combine(
                                value6,
                                Combine(value7, Combine(value8, Combine(value9, value10)))
                            )
                        )
                    )
                )
            )
        );

    /// <summary>Combine the hashes of each item in an array.</summary>
    public static int Array<T>(T[]? items)
    {
        if (items == null || items.Length == 0)
            return 0;

        int hash = items[0]?.GetHashCode() ?? 0;
        for (int i = 1; i < items.Length; i++)
            hash = Combine(hash, items[i]?.GetHashCode() ?? i);

        return hash;
    }
}
