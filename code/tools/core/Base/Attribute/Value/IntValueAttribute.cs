using System;

namespace Library
{
    public class IntValueAttribute : Attribute
    {
        public int value { get; protected set; }

        public IntValueAttribute(int value = 0)
        {
            this.value = value;
        }
    }
}