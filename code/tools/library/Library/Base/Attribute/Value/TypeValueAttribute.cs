using System;

namespace Library
{
    public class TypeValueAttribute : Attribute
    {
        public Type value { get; protected set; }

        public TypeValueAttribute(Type value = null)
        {
            this.value = value;
        }
    }
}