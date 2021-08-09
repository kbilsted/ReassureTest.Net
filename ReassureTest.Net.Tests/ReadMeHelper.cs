using System.IO;
using System.Reflection;
using KbgSoft.LineCounter;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class ReadMeHelper
    {
        [Test]
        public void MutateReadme()
        {
            var basePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "..", "..", "..", "..", "..");
            var linecounter = new LineCounting();
            linecounter.ReplaceWebshieldsInFile(basePath, Path.Combine(basePath, "README.md"));
        }
	}
}