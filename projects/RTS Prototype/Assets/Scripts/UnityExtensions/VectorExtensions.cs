using UnityEngine;

public static class VectorExtensions
{

    // HOW DO I DO THIS SHIIIIIT?
    /// <summary>
    /// Converts all the Coordinates of the Vector into positive Values
    /// </summary>
    public static void ConvertToPositiveCoord(this Vector3 v3)
    {

        if(v3.x < 0)
        {
            v3.x = -v3.x;
        }

        if(v3.y < 0)
        {
            v3.y = -v3.y;
        }

        if(v3.z < 0)
        {
            v3.z = -v3.z;
        }
    }

    /// <summary>
    /// Converts all the Coordinates of the Vector into positive Values
    /// </summary>
    public static Vector2 ConvertToPositiveCoord(this Vector2 v2)
    {
        Vector2 ReturnVal = new Vector2(v2.x, v2.y);
        if(v2.x < 0)
        {
            ReturnVal.Set(-v2.x, v2.y);
        }

        if(v2.y < 0)
        {
            ReturnVal.Set(v2.x, -v2.y);
        }

        return ReturnVal;
    }

    /// <summary>
    /// Sees if all of the Values of this Vector are the same as the given Value.
    /// </summary>
    public static bool AllValuesAreThis(this Vector2 v2, float compareValue)
    {
        if(v2.x == compareValue && v2.y == compareValue)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sees if all of the Values of this Vector are the same as the given Value.
    /// </summary>
    public static bool AllValuesAreThis(this Vector3 v3, float compareValue)
    {
        if(v3.x == compareValue && v3.y == compareValue && v3.z == compareValue)
        {
            return true;
        }
        return false;
    }
}

