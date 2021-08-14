using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
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
            ReplaceWebshieldsInFile(basePath, Path.Combine(basePath, "README.md"));
        }

        /// <summary>
        /// Looks for magic marker <!--start-->....<!--end--> and replace the body of that with the shield content
        /// </summary>
        public void ReplaceWebshieldsInFile(string codeFolderPath, string pathOfFileToMutate)
        {
            var stats = new LineCounting().CountFolder(codeFolderPath);

            var shieldsRegEx = new Regex("\r?\n<!--start-->[^<]*<!--end-->", RegexOptions.Singleline);
            var githubShields = new WebFormatter().CreateGithubShields(stats.Total);

            var oldReadme = File.ReadAllText(pathOfFileToMutate);
            var newReadMe = shieldsRegEx.Replace(oldReadme, $"\r\n<!--start-->\r\n{githubShields}<!--end-->");

            if (oldReadme != newReadMe)
                File.WriteAllText(pathOfFileToMutate, newReadMe);
        }
	}
}