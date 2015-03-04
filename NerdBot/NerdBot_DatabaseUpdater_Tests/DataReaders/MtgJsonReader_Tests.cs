using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Moq;
using NerdBot.Utilities;
using NerdBot_DatabaseUpdater.Mappers;
using NerdBot_DatabaseUpdater.MtgData;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot_DatabaseUpdater_Tests.DataReaders
{
    [TestFixture]
    class MtgJsonReader_Tests
    {
        private Mock<IMtgDataMapper<MtgJsonCard, MtgJsonSet>> dataMapperMock;
        private Mock<SearchUtility> searchUtilityMock;
        private Mock<IFileSystem> mFileSystemMock;
        private Mock<ILoggingService> mLoggingServiceMock;

        private string GetTestDataPath()
        {
            // Use the test assembly's directory instead of where nunit runs the test
            string outputPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            outputPath = outputPath.Replace("file:\\", "");
            return Path.Combine(outputPath, "Data");
        }
    }
}
