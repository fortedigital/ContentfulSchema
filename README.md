# Contentful Schema
![Build status](https://fortedigital.visualstudio.com/_apis/public/build/definitions/6b69c53b-f269-41dc-80b8-cd777360f810/9/badge)


Contentful Content Model generator based on .NET classes.

Installation
```
Install-Package Forte.ContentfulSchema

```

How to use

Create a content class with `ContentType` attribute. The attribute argument is an Id of the content type
and must be unique throughout the application.

```c#
[ContentType("article")]
public class Article
{
    [ContentTypeDisplayField]
    public string Title { get; set ;}
    
    public string Body { get; set; }
    
    public Asset Image { get; set; }
    
    public Author Author { get; set; }
} 


[ContentType("author")]
public class Author
{    
    public string GivenName { get; set; }
    
    [ContentTypeDisplayField]    
    public string FamilyName { get; set; }
} 
```

`ContentTypeDisplayField` attribute is used to mark a property that will be used as an `Display Field` in Contentful.
Only one field for a given content type should be decorated with this attribute.

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

It takes a type parameter that indicates an assembly in which content types should be looked for.

`SyncContentTypes` creates a new types and updates existing ones **but will not remove types that exists in Contentful but does not exist in code.**

**Library is compliant with .NET Standard 2.0**

## Features

### Localizable content

You can use `Localizable(true)` attribute on any property to mark them as localizable in Contentful.

### Reference content type validation

If you create a property in you content type that is a reference to another content type a link validator 
will be added. Inheritance is supported so its possible to assign the same content type and its descendands.

### Content type customization

Forte.ContentfulSchema provides a basic mapping between .NET types and Contentful types. If you want to change
that behaviour you can use/extend `ContentFieldTypeProvider` class or implement `IContentFieldTypeProvider` interface.
There is only one method that needs to be implemented `GetContentfulTypeForProperty`.
Default Contentful field types can be found in `SystemFieldTypes` class.

```c#
public interface IContentFieldTypeProvider
{
    string GetContentfulTypeForProperty(PropertyInfo property);
}
```

### Editor customization

Forte.ContentfulSchema provides basic editor configuration. If you want to change controls you should use/extend
`ContentEditorControlProvider` class or implement `IContentEditorControlProvider` interface. It has only one
method that takes C# PropertyInfo and Contentful Field as parameters and should return Contentful WidgetId.
Default widget ids can be found in `SystemWidgetIds` class.

```c#
public interface IContentEditorControlProvider
{
    string GetWidgetIdForField(PropertyInfo property, Field field);
}
```


