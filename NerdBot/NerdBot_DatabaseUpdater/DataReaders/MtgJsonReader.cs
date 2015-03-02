using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Mtg;
using NerdBot_DatabaseUpdater.Mappers;
using NerdBot_DatabaseUpdater.MtgData;
using SimpleLogging.Core;

namespace NerdBot_DatabaseUpdater.DataReaders
{
    public class MtgJsonReader : IMtgDataReader
    {
        private readonly IMtgDataMapper<MtgJsonCard, MtgJsonSet> mDataMapper;
        private readonly string mFileName;
        private readonly IFileSystem mFileSystem;
        private readonly ILoggingService mLoggingService;

        public MtgJsonReader(
            string fileName, 
            IMtgDataMapper<MtgJsonCard, MtgJsonSet> dataMapper,
            IFileSystem fileSystem,
            ILoggingService loggingService)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (dataMapper == null)
                throw new ArgumentNullException("dataMapper");

            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            if (!fileSystem.File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            this.mFileName = fileName;
            this.mDataMapper = dataMapper;
            this.mFileSystem = fileSystem;
            this.mLoggingService = loggingService;
        }

        public Set ReadSet()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> ReadCards()
        {
            throw new NotImplementedException();
        }
    }
}
