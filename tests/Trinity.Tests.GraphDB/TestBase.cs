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
using Semiodesk.Trinity.Store.GraphDB;
using System.IO;
using System.Reflection;
using System;

namespace Semiodesk.Trinity.Test.GraphDB
{
    public class TestBase
    {
        #region Members

        protected static string ConnectionString;

        protected Uri BaseUri;
        
        protected IStore Store;
        
        protected IModel Model1;

        protected IModel Model2;

        #endregion

        #region Methods

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            StoreFactory.LoadProvider<GraphDBStoreProvider>();
            OntologyDiscovery.AddAssembly(Assembly.GetExecutingAssembly());
            MappingDiscovery.RegisterAssembly(Assembly.GetExecutingAssembly());
            OntologyDiscovery.AddAssembly(typeof(AbstractMappingClass).Assembly);
            MappingDiscovery.RegisterAssembly(typeof(AbstractMappingClass).Assembly);

            var location = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var folder = new DirectoryInfo(Path.Combine(location.DirectoryName, "nunit"));

            if (folder.Exists)
            {
                folder.Delete(true);
            }

            folder.Create();
            
            ConnectionString = "provider=graphdb;host=http://localhost:7200;repository=trinity-test";
            
            BaseUri = new Uri("http://localhost:7200/repository/trinity-test/");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }
        
        [SetUp]
        public virtual void SetUp()
        {
            Store = StoreFactory.CreateStore(ConnectionString);
            
            Model1 = Store.GetModel(BaseUri.GetUriRef("model1"));
            
            if (!Model1.IsEmpty) Model1.Clear();
            
            Model2 = Store.GetModel(BaseUri.GetUriRef("model2"));
            
            if (!Model2.IsEmpty) Model2.Clear();
        }
        
        [TearDown]
        public void TearDown()
        {
            Model1.Clear();
            Model2.Clear();
            
            Store.Dispose();
        }

        protected Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            
            stream.Position = 0;
            
            return stream;
        }
        
        #endregion
    }
}
