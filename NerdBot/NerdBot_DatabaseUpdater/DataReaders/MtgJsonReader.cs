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

        private bool mSkipTokens = true;

        public bool SkipTokens
        {
            set { this.mSkipTokens = value; }
        }

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

            this.mLoggingService.Trace("JSON data = {0}", data);

            this.mLoggingService.Debug("Deserializing MtgJsonSet...");
            MtgJsonSet setData = JsonConvert.DeserializeObject<MtgJsonSet>(data, settings);
            this.mLoggingService.Debug("Deserialized MtgJsonSet!");

            // Get rarity quantities
            this.mLoggingService.Debug("Getting rarity quantities...");
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

            this.mLoggingService.Trace("Rarity quantifies: BasicLand = {0}; Common = {1}; Mythic = {2}; Uncommon = {3}; Rare = {4}",
                setData.BasicLandQty,
                setData.CommonQty,
                setData.MythicQty,
                setData.UncommonQty,
                setData.RareQty);

            this.mLoggingService.Debug("Mapping data...");
            var set = this.mDataMapper.GetSet(setData);
            this.mLoggingService.Debug("Data mapped for set '{0}'!", setData.Name);

            return set;
        }

        public IEnumerable<Card> ReadCards()
        {
            string data = this.mFileSystem.File.ReadAllText(this.mFileName);

            this.mLoggingService.Trace("JSON data = {0}", data);

            JObject cardObject = JObject.Parse(data);

            IList<JToken> results = cardObject["cards"].Children().ToList();

            var settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            // Read set info
            MtgJsonSet setData = JsonConvert.DeserializeObject<MtgJsonSet>(data, settings);

            foreach (JToken result in results)
            {
                this.mLoggingService.Debug("Deserializing MtgJsonCard...");
                MtgJsonCard cardData = JsonConvert.DeserializeObject<MtgJsonCard>(result.ToString(), settings);
                this.mLoggingService.Debug("MtgJsonCard deserialized!");

                if (this.mSkipTokens)
                {
                    if (cardData.Layout.ToLower() == "token")
                    {
                        this.mLoggingService.Debug("Card is a token, skipping...");

                        continue;
                    }
                }

                this.mLoggingService.Debug("Mapping data...");
                var card = this.mDataMapper.GetCard(cardData, setData.Name, setData.Code);
                this.mLoggingService.Debug("Data mapped for card '{0}'!", cardData.Name);

                yield return card;
            }
        }
    }
}
