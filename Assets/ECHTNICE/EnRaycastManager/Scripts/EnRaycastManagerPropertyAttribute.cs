using System;

[AttributeUsage(AttributeTargets.Field)]
public class EnRaycastManagerPropertyAttribute : Attribute {

    public string Json { get; set; }
    public EnRaycastManagerPropertyAttribute(string json)
    {
        Json = json;
    }
}
