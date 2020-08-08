using System;
using Library.Helper;

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

    public class IntValueAttribute : Attribute
    {
        public int value { get; protected set; }

        public IntValueAttribute(int value = 0)
        {
            this.value = value;
        }
    }

    public class StringValueAttribute : Attribute
    {
        public string value { get; protected set; }

        public StringValueAttribute(string value = "")
        {
            this.value = value;
        }
    }

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

    public class HookValueAttribute : StringValueAttribute
    {

    }
}