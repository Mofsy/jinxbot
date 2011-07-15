using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JinxBot.UpdateManager.Components.Tasks;
using System.IO;

namespace JinxBot.UpdateManager.Components.Tests
{
    [TestClass]
    public class SampleManifestReaderTest
    {
        [TestMethod]
        public void SampleManifestExecutesSuccessfully()
        {
            string path = @"F:\Projects\jinxbot\trunk\development\JinxBot.UpdateManager.Components\SampleManifest.xml";
            string xml = File.ReadAllText(path);

            ManifestHandler handler = new ManifestHandler(xml);

            Assert.AreEqual(TaskStatus.Success, handler.ExecuteManifest());
        }

        [TestCleanup]
        public void CleanupAfterTest()
        {
            Context.Reset();
        }
    }
}
