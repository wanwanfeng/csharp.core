using System;

namespace Library
{
    public class StringValueAttribute : Attribute
    {
        public string value { get; protected set; }

        public StringValueAttribute(string value = "")
        {
            this.value = value;
        }
    }
}