//
// https://github.com/NServiceKit/NServiceKit.Text
// NServiceKit.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;
using System.Collections.Generic;
using System.IO;

namespace NServiceKit.Text.Common
{
    /// <summary>Writes a list delegate.</summary>
    /// <param name="writer">    The writer.</param>
    /// <param name="oList">     The list.</param>
    /// <param name="toStringFn">to string function.</param>
    internal delegate void WriteListDelegate(TextWriter writer, object oList, WriteObjectDelegate toStringFn);

    /// <summary>Writes a generic list delegate.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="writer">    The writer.</param>
    /// <param name="list">      The list.</param>
    /// <param name="toStringFn">to string function.</param>
    internal delegate void WriteGenericListDelegate<T>(TextWriter writer, IList<T> list, WriteObjectDelegate toStringFn);

    /// <summary>Writes a delegate.</summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value"> The value.</param>
    internal delegate void WriteDelegate(TextWriter writer, object value);

    /// <summary>Parse factory delegate.</summary>
    /// <returns>A ParseStringDelegate.</returns>
    internal delegate ParseStringDelegate ParseFactoryDelegate();

    /// <summary>Writes an object delegate.</summary>
    /// <param name="writer">The writer.</param>
    /// <param name="obj">   The object.</param>
    public delegate void WriteObjectDelegate(TextWriter writer, object obj);

    /// <summary>Sets property delegate.</summary>
    /// <param name="instance">     The instance.</param>
    /// <param name="propertyValue">The property value.</param>
    public delegate void SetPropertyDelegate(object instance, object propertyValue);

    /// <summary>Parse string delegate.</summary>
    /// <param name="stringValue">The string value.</param>
    /// <returns>An object.</returns>
    public delegate object ParseStringDelegate(string stringValue);

    /// <summary>Convert object delegate.</summary>
    /// <param name="fromObject">from object.</param>
    /// <returns>An object.</returns>
    public delegate object ConvertObjectDelegate(object fromObject);

    /// <summary>Convert instance delegate.</summary>
    /// <param name="obj"> The object.</param>
    /// <param name="type">The type.</param>
    /// <returns>An object.</returns>
    public delegate object ConvertInstanceDelegate(object obj, Type type);
}
