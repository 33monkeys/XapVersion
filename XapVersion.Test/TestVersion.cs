using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace XapVersion.Test
{
    [TestFixture]
    public class TestVersion
    {
        [TestCase]
        public void GetVersion_Test()
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (var xap in XapFilesForTest)
                {
                    var path = Path.Combine("ClientBin", xap.FileName);
                    var version = VersionOf.XapFile(path);

                    Assert.That(version, Is.EqualTo(xap.Version));
                }
            }
        }

        public List<Xap> XapFilesForTest = new List<Xap>
        {
            new Xap {FileName = "SilverlightApplication1.xap", Version = "1.2.3"},
            new Xap {FileName = "SilverlightApplication2.xap", Version = "2.3.4"},
            new Xap {FileName = "SilverlightApplication3.xap", Version = "3.4.5"},
        };
    }
}
