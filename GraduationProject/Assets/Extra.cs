using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extra 
{
    public static int ToInt(this string s)
    {
        return int.Parse(s);
    }
}
