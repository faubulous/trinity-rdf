﻿// LICENSE:
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// AUTHORS:
//
//  Moritz Eberl <moritz@semiodesk.com>
//  Sebastian Faubel <sebastian@semiodesk.com>
//
// Copyright (c) Semiodesk GmbH 2015-2019

using System;

namespace Semiodesk.Trinity
{
    /// <summary>
    /// Decorate a property with this attribute to mark it as mapped RDF property with the given type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RdfPropertyAttribute : Attribute
    {
        #region Members
        /// <summary>
        /// Uri of the the RDF property
        /// </summary>
        public readonly Uri MappedUri;

        /// <summary>
        /// Flag determining if property is language invariant. Only valid for string or string collections.
        /// </summary>
        public bool LanguageInvariant;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uriString">There uri of the rdf property for this mapping.</param>
        /// <param name="languageInvariant">The mapping ignores the language setting and is always non-localized. Only valid if type or generic type is string or string collection.</param>
        public RdfPropertyAttribute(string uriString, bool languageInvariant = false)
        {
            MappedUri = new Uri(uriString);
            LanguageInvariant = languageInvariant;
        }


        #endregion
    }
}
