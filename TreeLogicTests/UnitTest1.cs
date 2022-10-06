using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TreeLogicN;


namespace TreeLogicTests
{
    public class Tests
    {
        List<string> paths;

        [SetUp]
        public void Setup()
        {
            string[] lines;
            lines = File.ReadAllLines(@"./../../../Tests/Paths.txt");
            paths = lines.ToList();
        }

        [Test]
        public void Test1()
        {
            Node nody = TreeLogic.createJSONTree("Papa", paths, '-', false);
            string trunk = "TRONCO";
            
            Assert.IsTrue(nody.Trunk == trunk, "Trunk name expected {0} : actual name {1}", trunk, nody.Trunk);
        }

        [Test]
        public void Test2()
        {
            Node nody = TreeLogic.createJSONTree("Papa", paths, '-', false);
            string trunk = "leave3";

            Assert.IsTrue(nody.Branches[0].Branches[0].Branches[2].Trunk == trunk, "Trunk name expected {0} : actual name {1}", trunk, nody.Trunk);
        }

        [Test]
        public void Test3()
        {
            Node nody = TreeLogic.createJSONTree("Papa", paths, '-', true);
            string trunk = "leave3";
            string trunk1 = "leave2";

            Assert.IsTrue(nody.Branches[2].Trunk == trunk, "Trunk name expected {0} : actual name {1}", trunk, nody.Trunk);

            Assert.IsTrue(nody.Branches[1].Trunk == trunk1, "Trunk name expected {0} : actual name {1}", trunk, nody.Trunk);
        }


        [Test]
        public void TestTrie()
        {
            List<List<string>> paths = new List<List<string>>();

            List<string> p = new List<string>();
            p.Add("HOLA");
            p.Add("Amigo");
            p.Add("HOLA");

            List<string> p1 = new List<string>();
            p1.Add("HOLA");
            p1.Add("Amigo");
            p1.Add("saludos");

            List<string> p2 = new List<string>();
            p2.Add("HOLA");
            p2.Add("Tonto");
            p2.Add("HEHE");

            paths.Add(p);
            paths.Add(p1);
            paths.Add(p2);

            TrieNode trie = new TrieNode(paths);

            Assert.IsTrue(trie.children["HOLA"].children.Count == 2);
        }
    }
}