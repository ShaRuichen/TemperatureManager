using System;

namespace Manager
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class SqlAttribute : Attribute
    {
        public string Name { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }
}
