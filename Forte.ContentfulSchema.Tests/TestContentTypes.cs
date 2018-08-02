using System.Collections.Generic;
using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;

namespace Forte.ContentfulSchema.Tests
{
    internal class NotContentType { }

    internal sealed class SealedNotContentType { }

    [ContentType("empty-type")]
    internal class EmptyContentType { }

    [ContentType("type-with-string")]
    internal class ContentTypeWithString
    {
        public string StringType { get; set; }
    }

    [ContentType("type-with-integer")]
    internal class ContentTypeWithInteger
    {
        public int IntegerProp { get; set; }
    }

    [ContentType("type-with-float")]
    internal class ContentTypeWithFloat
    {
        public float FloatProp { get; set; }
    }

    [ContentType("type-with-decimal")]
    internal class ContentTypeWithDecimal
    {
        public decimal DecimalProp { get; set; }
    }

    [ContentType("type-with-bool")]
    internal class ContentTypeWithBool
    {
        public bool BoolProp { get; set; }
    }

    [ContentType("type-with-char")]
    internal class ContentTypeWithChar
    {
        public char CharProp { get; set; }
    }

    [ContentType("type-with-byte")]
    internal class ContentTypeWithByte
    {
        public byte ByteProp { get; set; }
    }

    [ContentType("type-with-double")]
    internal class ContentTypeWithDouble
    {
        public double DoubleProp{ get; set; }
    }

    [ContentType("type-with-long")]
    internal class ContentTypeWithLong
    {
        public long LongProp{ get; set; }
    }

    [ContentType("type-with-sbyte")]
    internal class ContentTypeWithSByte
    {
        public sbyte SByteProp{ get; set; }
    }

    [ContentType("type-with-short")]
    internal class ContentTypeWithShort
    {
        public short ShortProp{ get; set; }
    }

    [ContentType("type-with-uint")]
    internal class ContentTypeWithUint
    {
        public uint UintProp{ get; set; }
    }

    [ContentType("type-with-ulong")]
    internal class ContentTypeWithULong
    {
        public ulong ULongProp{ get; set; }
    }

    [ContentType("type-with-ushort")]
    internal class ContentTypeWithUShort
    {
        public ushort UShortProp{ get; set; }
    }

    [ContentType("child-of-empty-type")]
    internal class ChildOfEmptyContentType : EmptyContentType
    {
        public string Prop { get; set; }
    }

    [ContentType("type-with-inherited-property")]
    internal class ContentTypeWithInheritedProperty : ContentTypeWithString { }

    [ContentType("grand-child-of-empty-type")]
    internal class GrandChildOfEmptyContentType : ChildOfEmptyContentType { }

    [ContentType("customized-content-type")]
    internal class CustomizedContentType : NotContentType
    {
        public string Prop { get; set; }
    }

    [ContentType("type-with-asset")]
    internal class ContentTypeWithAsset
    {
        public Asset Prop { get; set; }
    }

    [ContentType("type-with-entry-link")]
    internal class ContentTypeWithEntryLink
    {
        public Entry<NotContentType> LinkToNotContentType{ get; set; }
    }

    [ContentType("type-with-link")]
    internal class ContentTypeWithLink
    {
        public ContentTypeWithBool WithBoolType{ get; set; }
    }

    [ContentType("type-with-two-links")]
    internal class ContentTypeWithTwoLinks
    {
        public ContentTypeWithBool WithBoolType { get; set; }
        public ContentTypeWithInteger WithIntType { get; set; }
    }

    [ContentType("type-with-link-to-not-content-type")]
    internal class ContentTypeWithLinkToNotContentType
    {
        public NotContentType LinkToNotContentType { get; set; }
    }

    [ContentType("type-with-link-to-sealed-not-content-type")]
    internal class ContentTypeWithLinkToSealedNotContentType
    {
        public SealedNotContentType LinkToSealedNotContentType { get; set; }
    }

    [ContentType("type-with-inherited-link")]
    internal class ContentTypeWithInheritedLink : ContentTypeWithLink { }

    [ContentType("type-with-indirectly-inherited-link")]
    internal class ContentTypeWithIndirectlyInheritedLink : ContentTypeWithInheritedLink { }

    [ContentType("type-with-string-list")]
    internal class ContentTypeWithStringList
    {
        public List<string>  Strings{ get; set; }
    }

    [ContentType("type-with-string-array")]
    internal class ContentTypeWithStringArray
    {
        public string[] Strings{ get; set; }
    }

    [ContentType("type-with-generic-array")]
    internal class ContentTypeWithGenericArray
    {
        public ContentTypeWithString[] StringLinks{ get; set; }
    }

    [ContentType("type-with-generic-list")]
    internal class ContentTypeWithGenericList
    {
        public List<ContentTypeWithInteger> IntLinks{ get; set; }
    }
}
