using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace XapVersion.Test
{
    [TestFixture]
    public class TestVersion
    {
        [TestCase]
        [Repeat(100)]
        public void GetVersion_Test()
        {
            foreach (var xap in XapFilesForTest)
            {
                var path = Path.Combine("ClientBin", xap.FileName);
                var version = VersionOf.XapFile(path);

                Assert.That(version, Is.EqualTo(xap.Version));
            }
        }

        public List<Xap> XapFilesForTest = new List<Xap>
        {};
    }
}
