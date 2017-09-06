using System;

namespace ContentfulExt.Attributes
{
    public class DisplayAttribute : Attribute
    {
        public int Order { get; set; }

        public string Name { get; set; }

        public string Prompt { get; set; }
    }
}