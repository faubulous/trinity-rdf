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
// Copyright (c) Semiodesk GmbH 2023

using NUnit.Framework;
using Semiodesk.Trinity.Ontologies;
using Semiodesk.Trinity.Test.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System;

namespace Semiodesk.Trinity.Test.GraphDB
{
    [TestFixture]
    public class ModelTest : TestBase
    {
        #region Members
        
        private UriRef _r1;

        private UriRef _r2;

        private UriRef _r3;

        private Property _p1;

        private Property _p2;
        
        #endregion
        
        #region Methods
        
        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            
            Store.InitializeFromConfiguration();

            _r1 = BaseUri.GetUriRef("r1");
            _r2 = BaseUri.GetUriRef("r2");
            _r3 = BaseUri.GetUriRef("r3");
            
            _p1 = new Property(BaseUri.GetUriRef("p1"));
            _p2 = new Property(BaseUri.GetUriRef("p2"));
        }

        protected void InitializeModels()
        {
            var m1_r1 = Model1.CreateResource(_r1);
            m1_r1.AddProperty(_p1, "in the jungle");
            m1_r1.AddProperty(_p1, 123);
            m1_r1.AddProperty(_p1, DateTime.Now);
            m1_r1.Commit();

            var m1_r2 = Model1.CreateResource(_r2);
            m1_r2.AddProperty(_p1, "in the jungle");
            m1_r2.AddProperty(_p1, 123);
            m1_r2.AddProperty(_p1, DateTime.Now);
            m1_r2.Commit();
            
            var m2_r1 = Model2.CreateResource(_r1);
            m2_r1.AddProperty(_p1, "in the jungle");
            m2_r1.AddProperty(_p1, 123);
            m2_r1.AddProperty(_p1, DateTime.Now);
            m2_r1.Commit();

            var m2_r2 = Model2.CreateResource(_r2);
            m2_r2.AddProperty(_p1, "in the jungle");
            m2_r2.AddProperty(_p1, 123);
            m2_r2.AddProperty(_p1, DateTime.Now);
            m2_r2.Commit();
        }

        [Test]
        public void ModelNameTest()
        {
            InitializeModels();
            
            var model1 = Store.GetModel(BaseUri.GetUriRef("_model1"));
            model1.Clear();
            
            Assert.IsTrue(model1.IsEmpty);

            var model2 = Store.GetModel(BaseUri.GetUriRef("_model2"));
            model2.Clear();

            Assert.IsTrue(model2.IsEmpty);
            
            var m1_r1 = model1.CreateResource<PersonContact>(BaseUri.GetUriRef("r1"));
            m1_r1.NameFamily = "Doe";
            m1_r1.Commit();

            Assert.IsFalse(model1.IsEmpty);
            Assert.IsTrue(model2.IsEmpty);

            model1.Clear();

            Assert.IsTrue(model1.IsEmpty);
            Assert.IsTrue(model2.IsEmpty);
        }

        [Test]
        public void ContainsResourceTest()
        {
            InitializeModels();
            
            Assert.IsTrue(Model1.ContainsResource(_r1));
            Assert.IsTrue(Model1.ContainsResource(_r2));
            
            Assert.IsTrue(Model2.ContainsResource(_r1));
            Assert.IsTrue(Model2.ContainsResource(_r2));
        }

        [Test]
        public void CreateEmptyResourceTest()
        {
            var empty = Model1.CreateResource(BaseUri.GetUriRef("empty"));
            empty.Commit();
        }
        
        [Test]
        public void CreateReferencedResourcesTest()
        {
            Assert.IsFalse(Model1.ContainsResource(_r1));
            Assert.IsFalse(Model1.ContainsResource(_r2));
            
            var r2 = Model1.CreateResource(_r2);
            r2.AddProperty(_p1, 123);
            r2.AddProperty(_p2, new Resource(_r1));
            r2.Commit();

            Assert.IsTrue(Model1.ContainsResource(_r1));
            Assert.IsTrue(Model1.ContainsResource(_r2));
        }

        [Test]
        public void DeleteResourceTest()
        {
            InitializeModels();
            
            Assert.IsTrue(Model1.ContainsResource(_r1));

            Model1.DeleteResource(_r1);

            Assert.IsFalse(Model1.ContainsResource(_r1));
            
            var r0 = Model1.CreateResource(_r1);
            r0.AddProperty(_p1, "in the jungle");
            r0.AddProperty(_p1, 123);
            r0.Commit();

            var r3 = Model1.CreateResource(_r3);
            r3.AddProperty(_p1, 123);
            r3.AddProperty(_p2, r0);
            r3.Commit();

            Assert.IsTrue(Model1.ContainsResource(r0));
            Assert.IsTrue(Model1.ContainsResource(r3));

            Model1.DeleteResource(r0);

            Assert.IsFalse(Model1.ContainsResource(r0));
            Assert.IsTrue(Model1.ContainsResource(r3));

            // Update the resource from the model.
            r3 = Model1.GetResource(r3);

            Assert.IsTrue(r3.HasProperty(_p1, 123));
            Assert.IsFalse(r3.HasProperty(_p2, r0));
        }

        [Test]
        public void DeleteResourcesTest()
        {
            InitializeModels();
            
            Assert.IsTrue(Model1.ContainsResource(_r1));
            Assert.IsTrue(Model1.ContainsResource(_r2));
            
            var r1 = Model1.GetResource(_r1);
            var r2 = Model1.GetResource(_r2);

            Model1.DeleteResources(null, r1, r2);

            Assert.IsFalse(Model1.ContainsResource(_r1));
            Assert.IsFalse(Model1.ContainsResource(_r2));
        }

        [Test]
        public void DeleteResourcesFromUrisTest()
        {
            InitializeModels();
            
            Assert.IsTrue(Model1.ContainsResource(_r1));
            Assert.IsTrue(Model1.ContainsResource(_r2));

            Model1.DeleteResources(new Uri[] { _r1, _r2 });

            Assert.IsFalse(Model1.ContainsResource(_r1));
            Assert.IsFalse(Model1.ContainsResource(_r2));
        }

        [Test]
        public void GetResourceWithBlankIdTest()
        {
            var rX = Model1.CreateResource(new BlankId());
            rX.AddProperty(_p1, 123);
            rX.Commit();

            Assert.Throws<ArgumentException>(() => Model1.GetResource<Resource>(rX.Uri));
        }

        [Test]
        public void GetResourceWithBlankIdPropertyTest()
        {
            var r0 = Model1.CreateResource(new UriRef("_:0", true));
            r0.AddProperty(_p1, "0");
            r0.Commit();

            var r2 = Model1.CreateResource(new UriRef("_:1", true));
            r0.AddProperty(_p1, "1");
            r2.AddProperty(_p2, r0);
            r2.Commit();

            Assert.Throws<ArgumentException>(() => Model1.ContainsResource(r2.Uri));
            Assert.Throws<ArgumentException>(() => Model1.GetResource(r2.Uri));
            Assert.Throws<ArgumentException>(() => Model1.GetResource(r2));

            var results = Model1.GetResources<Resource>().ToArray();

            Assert.AreEqual(2, results.Length);

            foreach (var r in results)
            {
                Assert.IsTrue(r.Uri.IsBlankId);

                foreach(var x in r.ListValues(_p2).OfType<Resource>())
                {
                    Assert.IsTrue(x.Uri.IsBlankId);
                }
            }
        }
        
        [Test]
        public void GetResourceWithDuplicateBlankIdPropertyTest()
        {
            var r0 = Model1.CreateResource(new UriRef("_:0", true));
            r0.AddProperty(_p1, "0");
            r0.Commit();

            var r1 = Model1.CreateResource(new UriRef("_:0", true));
            r0.AddProperty(_p1, "1");
            r1.AddProperty(_p2, r0);
            r1.Commit();

            Assert.Throws<ArgumentException>(() => Model1.ContainsResource(r1.Uri));
            Assert.Throws<ArgumentException>(() => Model1.GetResource(r1.Uri));
            Assert.Throws<ArgumentException>(() => Model1.GetResource(r1));

            var resources = Model1.GetResources<Resource>().ToArray();

            Assert.AreEqual(2, resources.Length);

            foreach (var r in resources)
            {
                Assert.IsTrue(r.Uri.IsBlankId);

                foreach (var x in r.ListValues(_p2).OfType<Resource>())
                {
                    Assert.IsTrue(x.Uri.IsBlankId);
                }
            }
        }

        [Test]
        public void GetResourceTest()
        {
            InitializeModels();
            
            var r1 = Model1.GetResource(_r1);
            
            Assert.NotNull(r1);
            Assert.NotNull(r1.Model);

            r1 = Model1.GetResource<Resource>(_r1);
            
            Assert.NotNull(r1);
            Assert.NotNull(r1.Model);

            r1 = Model1.GetResource(_r1, typeof(Resource)) as Resource;
            
            Assert.NotNull(r1);
            Assert.NotNull(r1.Model);

            try
            {
                var x1 = BaseUri.GetUriRef("x1");
                
                Model1.GetResource<Resource>(x1);

                Assert.Fail();
            }
            catch(ArgumentException)
            {
            }
        }

        [Test]
        public void GetResourcesTest()
        {
            InitializeModels();
            
            Assert.IsTrue(Model1.ContainsResource(_r1));
            
            var query = new SparqlQuery("DESCRIBE @resource").Bind("@resource", _r1);
            var results = Model1.GetResources(query).ToList();

            Assert.AreEqual(1, results.Count);
            Assert.NotNull(results[0].Model);
        }

        [Test]
        public void UpdateResourceTest()
        {
            InitializeModels();
            
            var r1 = Model1.GetResource(_r1);
            r1.RemoveProperty(_p1, 123);
            r1.Commit();

            var actual = Model1.GetResource(_r1);

            Assert.AreEqual(r1, actual);

            actual = Model1.GetResource<Resource>(_r1);

            Assert.AreEqual(r1, actual);

            // Try to update resource with different properties then persisted..
            var r1mod = new Resource(_r1);
            r1mod.Model = Model1;
            r1mod.AddProperty(_p1, "in the jengle");
            r1mod.Commit();
            
            actual = Model1.GetResource<Resource>(_r1);
            
            Assert.AreEqual(r1mod, actual);
        }

        [Test]
        public void DateTimeResourceTest()
        {
            Assert.IsTrue(DateTime.TryParse("2013-01-21T16:27:23.000Z", out var t));
            
            var r1 = Model1.CreateResource(_r1);
            r1.AddProperty(_p1, t);
            r1.Commit();

            var actual = Model1.GetResource(_r1);
            var v = (DateTime)actual.GetValue(_p1);

            Assert.AreEqual(t.ToUniversalTime(), v.ToUniversalTime());
        }

        [Test]
        public void TimeSpanResourceTest()
        {
            var t = TimeSpan.FromMinutes(5);
            
            var r1 = Model1.CreateResource(_r1);
            r1.AddProperty(_p1, t);
            r1.Commit();

            var actual = Model1.GetResource(_r1);
            var v = (TimeSpan)actual.GetValue(_p1);

            Assert.AreEqual(t.TotalMinutes, v.TotalMinutes);
        }

        [Test]
        public void LiteralWithHyphenTest()
        {
            var r1 = Model1.CreateResource(_r1);
            r1.AddProperty(_p1, "\"in the jungle\"");
            r1.Commit();

            var actual = Model1.GetResource(_r1);
            var v = (string)actual.GetValue(_p1);
            
            Assert.AreEqual("\"in the jungle\"", v);
        }

        [Test]
        public void LiteralWithLangTagTest()
        {
            var r1 = Model1.CreateResource(_r1);
            r1.AddProperty(_p1, "in the jungle", "en");
            r1.Commit();

            var actual = Model1.GetResource(_r1);
            var v = (Tuple<string, string>)actual.GetValue(_p1);

            Assert.AreEqual("in the jungle", v.Item1);
            Assert.AreEqual("en", v.Item2);
        }

        [Test]
        public void LiteralWithNewLineTest()
        {
            var r1 = Model1.CreateResource(_r1);
            r1.AddProperty(_p1, "in the\n jungle");
            r1.Commit();

            var actual = Model1.GetResource(_r1);
            var v = (string)actual.GetValue(_p1);
            
            Assert.AreEqual("in the\n jungle", v);
        }

        [Test]
        public void AddResourceTest()
        {
            var r1 = new Resource (_r1);
            r1.AddProperty(_p1, "in the jungle");
            r1.AddProperty(_p1, 123);
            r1.AddProperty(_p1, DateTime.Now);

            Model1.AddResource(r1);

            var actual = Model1.GetResource(_r1);

            Assert.AreEqual(_r1, actual.Uri);
            Assert.AreEqual(r1.ListValues(_p1).Count(), actual.ListValues(_p1).Count());
            
            var r2 = new Contact(_r2);
            r2.Fullname = "Peter";

            Model1.AddResource(r2);

            var actual2 = Model1.GetResource<Contact>(_r2);

            Assert.AreEqual(_r2, actual2.Uri);
            Assert.AreEqual(r2.Fullname, actual2.Fullname);
        }

        [Test]
        public void GetTypedResourcesTest()
        {
            var r1 = Model1.CreateResource<Contact>(_r1);
            r1.Fullname = "Peter";
            r1.Commit();

            var r2 = Model1.CreateResource<Contact>(_r2);
            r2.Fullname = "Hans";
            r2.Commit();

            var results = Model1.GetResources<Contact>().ToList();

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.Contains(r1));
            Assert.IsTrue(results.Contains(r2));

            Model1.Clear();

            var r3 = Model1.CreateResource<PersonContact>(_r3);
            r3.Fullname = "Peter";
            r3.Commit();

            results = Model1.GetResources<Contact>().ToList();
            
            Assert.AreEqual(0, results.Count);

            results = Model1.GetResources<Contact>(true).ToList();
            
            Assert.AreEqual(1, results.Count);

            var actual = Model1.GetResource(_r1);

            Assert.AreEqual(typeof(PersonContact), actual.GetType());
        }

        [Test]
        public void WriteTest()
        {
            var r1 = Model1.CreateResource(_r1);
            r1.AddProperty(_p1, "in the\n jungle");
            r1.Commit();

            var stream = new MemoryStream();
            
            Model1.Write(stream, RdfSerializationFormat.RdfXml);
            
            var result = Encoding.UTF8.GetString(stream.ToArray());
            
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [Test]
        public void WriteWithBaseUriTest()
        {
            var r1 = Model1.CreateResource(_r1);
            r1.AddProperty(_p1, "test");
            r1.Commit();

            using (var s = new MemoryStream())
            {
                Model1.Write(s, RdfSerializationFormat.Turtle, null, BaseUri, true);

                s.Seek(0, SeekOrigin.Begin);

                var result = Encoding.UTF8.GetString(s.ToArray());

                Assert.IsFalse(string.IsNullOrEmpty(result));
                Assert.IsTrue(result.StartsWith("@base <" + BaseUri.OriginalString + ">"));
            }
        }

        [Test]
        public void ReadTest()
        {
            var file = new FileInfo("Models\\test-ntriples.nt");
            var fileUri = file.ToUriRef();
            
            Assert.IsTrue(Model1.Read(fileUri, RdfSerializationFormat.NTriples, false));
            Assert.IsFalse(Model1.IsEmpty);

            Model1.Clear();
            
            Assert.IsTrue(Model1.Read(rdf.Namespace, RdfSerializationFormat.RdfXml, false));
            Assert.IsFalse(Model1.IsEmpty);

            Model1.Clear();

            file = new FileInfo("Models\\test-tmo.trig");
            fileUri = file.ToUriRef();
            
            Assert.Throws(typeof(ArgumentException), () => { Model1.Read(fileUri, RdfSerializationFormat.Trig, false); });

        }

        [Test]
        public void ReadFromStringTest()
        {
            var data = @"
@base <http://example.org/> .
@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .
@prefix foaf: <http://xmlns.com/foaf/0.1/> .
@prefix rel: <http://www.perceive.net/schemas/relationship/> .

<#green-goblin>
    rel:enemyOf <#spiderman> ;
    a foaf:Person ;    # in the context of the Marvel universe
    foaf:name ""Green Goblin"" .

<#spiderman>
    rel:enemyOf <#green-goblin> ;
    a foaf:Person ;
    foaf:name ""Spiderman"", ""Человек-паук""@ru .
";

            using (var s1 = GenerateStreamFromString(data))
            {
                Assert.IsTrue(Model1.Read(s1, RdfSerializationFormat.Turtle, false));
            }

            var r1 = Model1.GetResource(new Uri("http://example.org/#green-goblin"));
            var v1 = (string)r1.GetValue(foaf.name);
            
            Assert.AreEqual("Green Goblin", v1);

            var data2 = @"
@base <http://example.org/> .
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix foaf: <http://xmlns.com/foaf/0.1/> .

<#green-goblin> foaf:age ""27""^^xsd:int .
";

            using (Stream s2 = GenerateStreamFromString(data2))
            {
                Assert.IsTrue(Model1.Read(s2, RdfSerializationFormat.Turtle, true));
            }

            var r2 = Model1.GetResource(new Uri("http://example.org/#green-goblin"));
            var v2 = (int)r2.GetValue(foaf.age);
            
            Assert.AreEqual(27, v2);

            var data3 = @"
@base <http://example.org/> .
@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .
@prefix foaf: <http://xmlns.com/foaf/0.1/> .
@prefix rel: <http://www.perceive.net/schemas/relationship/> .

<#green-goblin>
    rel:enemyOf <#spiderman> ;
    a foaf:Person ;    # in the context of the Marvel universe
    foaf:name ""Green Gobo"" .

<#spiderman>
    rel:enemyOf <#green-goblin> ;
    a foaf:Person ;
    foaf:name ""Spiderman"", ""Человек-паук""@ru .
";

            using (Stream s3 = GenerateStreamFromString(data))
            {
                Assert.IsTrue(Model1.Read(s3, RdfSerializationFormat.Turtle, false));
            }

            var r3 = Model1.GetResource(new Uri("http://example.org/#green-goblin"));
            var v3 = (string)r3.GetValue(foaf.name);
            
            Assert.AreEqual("Green Gobo", v3);
        }

        [Test]
        public void WriteToStringTest()
        {
            var r1 = Model1.CreateResource(_r1);
            r1.AddProperty(_p1, "test");
            r1.Commit();

            using (var s = new MemoryStream())
            {
                Model1.Write(s, RdfSerializationFormat.Turtle, null, null, true);

                s.Seek(0, SeekOrigin.Begin);
                
                var result = Encoding.UTF8.GetString(s.ToArray());

                Assert.IsFalse(string.IsNullOrEmpty(result));   
            }
        }
        
        #endregion
    }
}
