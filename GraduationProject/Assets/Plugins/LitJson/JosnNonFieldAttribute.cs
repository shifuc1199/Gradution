using UnityEngine;
using System.Collections;
using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class JsonNonFieldAttribute : Attribute
{
    public JsonNonFieldAttribute()
    {
    }
}