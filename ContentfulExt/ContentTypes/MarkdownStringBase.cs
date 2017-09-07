using System.IO;
using System.Text.Encodings.Web;
using CommonMark;
using CommonMark.Formatters;
using CommonMark.Syntax;
using Newtonsoft.Json;

namespace ContentfulExt.ContentTypes
{
    public abstract class MarkdownStringBase : WrappedString, IMarkdownString
    {
        private static CommonMarkSettings markdownSettings = CreateMarkdownSettings();

        private static CommonMarkSettings CreateMarkdownSettings()
        {
            var settings = CommonMarkSettings.Default.Clone();
            settings.RenderSoftLineBreaksAsLineBreaks = true;
            settings.OutputDelegate = (doc, output, s) => new PrettyPrintOutputFormat(output, s).WriteDocument(doc);
            return settings;
        }

        public MarkdownStringBase(string value) : base(value){}

        private class PrettyPrintOutputFormat : HtmlFormatter
        {
            public PrettyPrintOutputFormat(TextWriter target, CommonMarkSettings settings) : base(target, settings)
            {
            }

            protected override void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildNodes)
            {
                if (block.Tag == BlockTag.IndentedCode)
                {
                    ignoreChildNodes = true;

                    EnsureNewLine();
                    Write("<pre class=\"prettyprint\"><code");

                    var info = block.FencedCodeData == null ? null : block.FencedCodeData.Info;
                    if (info != null && info.Length > 0)
                    {
                        var x = info.IndexOf(' ');
                        if (x == -1)
                            x = info.Length;

                        Write(" class=\"language-");
                        WriteEncodedHtml(info.Substring(0, x));
                        Write('\"');
                    }
                    Write('>');
                    WriteEncodedHtml(block.StringContent);
                    WriteLine("</code></pre>");
                }
                else
                {
                    base.WriteBlock(block, isOpening, isClosing, out ignoreChildNodes);
                }
            }
        }
    }
}