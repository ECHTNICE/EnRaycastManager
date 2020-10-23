using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public sealed class EnReadOnlyAttribute : PropertyAttribute {
}