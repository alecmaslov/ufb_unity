using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public static class ObjectExtensions
{
    private const int MaxListItems = 3; // Maximum number of list or array items to print
    private const int MaxDepth = 5; // Maximum depth of recursion

    public static string ToDetailedString(this object obj, int currentDepth = 0)
    {
        if (currentDepth > MaxDepth)
            return "...";

        if (obj == null)
            return "null";

        Type objType = obj.GetType();

        // Handle basic types
        if (objType.IsPrimitive || objType == typeof(string))
            return obj.ToString();

        // Handle lists or arrays
        if (obj is IEnumerable enumerable)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            int count = 0;
            foreach (var item in enumerable)
            {
                if (count++ >= MaxListItems)
                {
                    sb.Append(", ...");
                    break;
                }

                sb.Append(item.ToDetailedString(currentDepth + 1));
                sb.Append(", ");
            }

            if (count > 0)
                sb.Length -= 2; // Remove the last comma and space

            sb.Append("]");
            return sb.ToString();
        }

        // Handle objects
        StringBuilder objSb = new StringBuilder();
        objSb.Append(objType.Name + " { ");

        foreach (var field in objType.GetFields())
        {
            if (field.IsStatic)
                continue; // Skip static fields

            var value = field.GetValue(obj);
            objSb.Append($"{field.Name}: {value.ToDetailedString(currentDepth + 1)}, ");
        }

        if (objType.GetFields().Length > 0)
            objSb.Length -= 2; // Remove the last comma and space

        objSb.Append(" }");
        return objSb.ToString();
    }
}
