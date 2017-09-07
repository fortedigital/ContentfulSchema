using System.IO;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace Forte.ContentfulSchema.ContentTypes
{
    public abstract class LongStringBase : WrappedString, ILongString
    {
        public LongStringBase(string value) : base(value) {}

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            var lines = this.Value.Split('\n');
            int i;
            for (i = 0; i < lines.Length - 1; i++)
            {
                writer.Write(EncodeLine(lines[i], encoder));
                writer.Write("<br/>");           
            }
            writer.Write(EncodeLine(lines[i], encoder));
        }

        private static string EncodeLine(string line, HtmlEncoder encoder)
        {
            return Regex.Replace(encoder.Encode(line), "(^| )([^ ]+@[^ ]+\\.[^ ]{2,3})($| )", "<a href=\"mailto:$2\">$2</a>");
        }
    }
}