namespace ContentfulExt.ContentTypes
{
    public class WrappedString
    {
        protected readonly string Value;

        public WrappedString(string value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Value ?? "";        
        } 
    }
}