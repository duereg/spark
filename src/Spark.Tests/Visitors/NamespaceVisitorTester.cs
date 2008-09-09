﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Spark.Compiler.NodeVisitors;
using Spark.Parser.Markup;
using Spark.Tests.Visitors;

namespace Spark.Tests.Visitors
{
    [TestFixture]
    public class NamespaceVisitorTester : BaseVisitorTester
    {
        [Test]
        public void AssignNamespaceToElement()
        {
            var nodes = ParseNodes("<foo xmlns:x='http://spark.dejardin.org/x'><x:bar/></foo>");
            var visitor = new NamespaceVisitor(new VisitorContext());
            visitor.Accept(nodes);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[0]).Namespace);
            Assert.AreEqual("http://spark.dejardin.org/x", ((ElementNode)visitor.Nodes[1]).Namespace);
        }

        [Test]
        public void AssignNamespaceWithDefaultPrefix()
        {
            var nodes = ParseNodes("<foo><quux:bar/></foo>");
            var visitor = new NamespaceVisitor(new VisitorContext { Prefix = "quux" });
            visitor.Accept(nodes);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[0]).Namespace);
            Assert.AreEqual("http://spark.dejardin.org/", ((ElementNode)visitor.Nodes[1]).Namespace);
        }

        [Test]
        public void AssignNamespaceToAttributes()
        {
            var nodes = ParseNodes("<foo xmlns:x='http://spark.dejardin.org/x'><bar x:if='false'/></foo>");
            var visitor = new NamespaceVisitor(new VisitorContext());
            visitor.Accept(nodes);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[0]).Namespace);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[1]).Namespace);

            Assert.AreEqual("http://spark.dejardin.org/x", ((ElementNode)visitor.Nodes[1]).Attributes[0].Namespace);
        }

        [Test]
        public void ElementCanUseXmlnsOnSelf()
        {
            var nodes = ParseNodes("<x:foo y:bar='hello' xmlns:x='http://spark.dejardin.org/x' xmlns:y='http://spark.dejardin.org/y'><quux/></foo>");
            var visitor = new NamespaceVisitor(new VisitorContext());
            visitor.Accept(nodes);
            Assert.AreEqual("http://spark.dejardin.org/x", ((ElementNode)visitor.Nodes[0]).Namespace);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[1]).Namespace);

            Assert.AreEqual("y:bar", ((ElementNode)visitor.Nodes[0]).Attributes[0].Name);
            Assert.AreEqual("http://spark.dejardin.org/y", ((ElementNode)visitor.Nodes[0]).Attributes[0].Namespace);

        }

        [Test]
        public void ScopeOfXmlnsIsLimited()
        {
            var nodes = ParseNodes("<x:pre/><x:foo xmlns:x='http://spark.dejardin.org/x'/><x:post/>");
            var visitor = new NamespaceVisitor(new VisitorContext());
            visitor.Accept(nodes);
            Assert.AreEqual("x:pre", ((ElementNode)visitor.Nodes[0]).Name);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[0]).Namespace);
            Assert.AreEqual("x:foo", ((ElementNode)visitor.Nodes[1]).Name);
            Assert.AreEqual("http://spark.dejardin.org/x", ((ElementNode)visitor.Nodes[1]).Namespace);
            Assert.AreEqual("x:post", ((ElementNode)visitor.Nodes[2]).Name);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[2]).Namespace);

            nodes = ParseNodes("<x:pre/><x:foo xmlns:x='http://spark.dejardin.org/x'><x:bar/></x:foo><x:post/>");
            visitor = new NamespaceVisitor(new VisitorContext());
            visitor.Accept(nodes);
            Assert.AreEqual("x:pre", ((ElementNode)visitor.Nodes[0]).Name);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[0]).Namespace);
            Assert.AreEqual("x:foo", ((ElementNode)visitor.Nodes[1]).Name);
            Assert.AreEqual("http://spark.dejardin.org/x", ((ElementNode)visitor.Nodes[1]).Namespace);
            Assert.AreEqual("x:bar", ((ElementNode)visitor.Nodes[2]).Name);
            Assert.AreEqual("http://spark.dejardin.org/x", ((ElementNode)visitor.Nodes[2]).Namespace);
            Assert.AreEqual("x:post", ((ElementNode)visitor.Nodes[4]).Name);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[4]).Namespace);
        }

        [Test]
        public void NestedElementsDontWreckScope()
        {
            var nodes = ParseNodes("<x:pre/><x:foo xmlns:x='http://spark.dejardin.org/x'><x:foo><x:foo/></x:foo></x:foo><x:post/>");
            var visitor = new NamespaceVisitor(new VisitorContext());
            visitor.Accept(nodes);
            Assert.AreEqual("x:pre", ((ElementNode)visitor.Nodes[0]).Name);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[0]).Namespace);
            Assert.AreEqual("x:foo", ((ElementNode)visitor.Nodes[1]).Name);
            Assert.AreEqual("http://spark.dejardin.org/x", ((ElementNode)visitor.Nodes[1]).Namespace);
            Assert.AreEqual("x:foo", ((ElementNode)visitor.Nodes[2]).Name);
            Assert.AreEqual("http://spark.dejardin.org/x", ((ElementNode)visitor.Nodes[2]).Namespace);
            Assert.AreEqual("x:foo", ((ElementNode)visitor.Nodes[3]).Name);
            Assert.AreEqual("http://spark.dejardin.org/x", ((ElementNode)visitor.Nodes[3]).Namespace);
            Assert.AreEqual("x:foo", ((EndElementNode)visitor.Nodes[4]).Name);
            Assert.AreEqual("http://spark.dejardin.org/x", ((EndElementNode)visitor.Nodes[4]).Namespace);
            Assert.AreEqual("x:foo", ((EndElementNode)visitor.Nodes[5]).Name);
            Assert.AreEqual("http://spark.dejardin.org/x", ((EndElementNode)visitor.Nodes[5]).Namespace);
            Assert.AreEqual("x:post", ((ElementNode)visitor.Nodes[6]).Name);
            Assert.AreEqual("", ((ElementNode)visitor.Nodes[6]).Namespace);

        }
    }
}
