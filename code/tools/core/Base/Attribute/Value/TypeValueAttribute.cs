using System;

namespace Library
{
    public class TypeValueAttribute : Attribute
    {
        public Type value { get; protected set; }
        public object arg { get; protected set; }

        public TypeValueAttribute(Type value = null, object arg = null)
        {
            this.value = value;
            this.arg = arg;
        }
    }
}