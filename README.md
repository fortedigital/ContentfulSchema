# Contentful Schema

Contentful Content Model generator based on .NET classes.

Installation
```
Install-Package Forte.ContentfulSchema

```

How to use

Create a content class with `ContentType` attribute and inherit `ContentModelBase` class.

```c#
[ContentType("article")]
public class Article : ContentModelBase
{
    [ContentTypeDisplayField]
    public string Title { get; set ;}
    
    public string Body { get; set; }
    
    public Asset Image { get; set; }
    
    public Author Author { get; set; }
} 


[ContentType("author")]
public class Author : ContentModelBase
{    
    public string GivenName { get; set; }
    
    [ContentTypeDisplayField]    
    public string FamilyName { get; set; }
} 
```

Attribute `ContentTypeDisplayField` is used to mark a property that will be used as an `Display Field` in Contentful.

Then in `Startup` file run content synchronization:

```c#
public void ConfigureServices(IServiceCollection services)
{
    using (var httpClient = new HttpClient())
    {
        IContentfulManagementClient cfManagementClient = new ContentfulManagementClient(httpClient, new ContentfulOptions
        {
            ManagementApiKey = "<management_key>",
            SpaceId = "<space_id>",
            DeliveryApiKey = "<delivery_api_key>"
        });
        
        cfManagementClient.SyncContentTypes<Startup>().GetAwaiter().GetResult();
    }
}
```

`Forte.ContentfulSchema` adds an extension method `SyncContentTypes<T>` that calls the Contentful API and synchronizes content types between Contentufl and code.

It get a type parameter that indicates an assembly in which content types should be looked for.

`SyncContentTypes` creates a new types and updates existing ones **but will not remove types that exist in Contentful but does not exist in a code**

