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
    }
}