using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumFlagsAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class RangeEvenAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;

        public RangeEvenAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class RangeUnevenAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;

        public RangeUnevenAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}