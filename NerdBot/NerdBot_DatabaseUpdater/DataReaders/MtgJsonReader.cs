using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using NerdBot.Mtg;
using NerdBot_DatabaseUpdater.Mappers;
using NerdBot_DatabaseUpdater.MtgData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            string data = this.mFileSystem.File.ReadAllText(this.mFileName);

            var settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            MtgJsonSet setData = JsonConvert.DeserializeObject<MtgJsonSet>(data, settings);

            // Get rarity quantities
            JObject cardObject = JObject.Parse(data);
            IList<JToken> results = cardObject["cards"].Children().ToList();

            foreach (JToken result in results)
            {
                string rarity = result["rarity"].ToString();

                switch (rarity.ToLower())
                {
                    case "basic land":
                        setData.BasicLandQty += 1;
                        break;
                    case "common":
                        setData.CommonQty += 1;
                        break;
                    case "mythic":
                    case "mythic rare":
                        setData.MythicQty += 1;
                        break;
                    case "uncommon":
                        setData.UncommonQty += 1;
                        break;
                    case "rare":
                        setData.RareQty += 1;
                        break;
                }
            }

            var set = this.mDataMapper.GetSet(setData);

            return set;
        }

        public IEnumerable<Card> ReadCards()
        {
            string data = this.mFileSystem.File.ReadAllText(this.mFileName);

            JObject cardObject = JObject.Parse(data);

            IList<JToken> results = cardObject["cards"].Children().ToList();

            var settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            // Read set info
            MtgJsonSet setData = JsonConvert.DeserializeObject<MtgJsonSet>(data, settings);

            foreach (JToken result in results)
            {
                MtgJsonCard cardData = JsonConvert.DeserializeObject<MtgJsonCard>(result.ToString(), settings);

                var card = this.mDataMapper.GetCard(cardData, setData.Name, setData.Code);

                yield return card;
            }
        }
    }
}
