namespace Forte.ContentfulSchema.Discovery
{
    public interface IContentTree
    {
        IContentNode GetNodeByContentTypeId(string contentTypeId);
    }
}
