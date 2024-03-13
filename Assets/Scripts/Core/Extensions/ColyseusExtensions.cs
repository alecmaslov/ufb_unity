using Colyseus.Schema;

public static class ColyseusExtensions
{
    public static T[] ToArray<T>(this ArraySchema<T> arraySchema)
    {
        var array = new T[arraySchema.Count];
        for (var i = 0; i < arraySchema.Count; i++)
        {
            array[i] = arraySchema[i];
        }
        return array;
    }

    public static bool[] ToBoolArray(this ArraySchema<byte> arraySchema)
    {
        var array = new bool[arraySchema.Count];
        for (var i = 0; i < arraySchema.Count; i++)
        {
            array[i] = arraySchema[i] == 1;
        }
        return array;
    }
}