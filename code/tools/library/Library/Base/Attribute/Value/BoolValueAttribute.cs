using System;

namespace Library
{
    public class BoolValueAttribute : Attribute
    {
        public bool value { get; protected set; }

        public BoolValueAttribute(bool value = false)
        {
            this.value = value;
        }
    }
}