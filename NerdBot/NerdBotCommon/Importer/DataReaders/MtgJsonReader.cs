using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Importer.Mapper;
using NerdBotCommon.Importer.MtgData;
using NerdBotCommon.Mtg;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleLogging.Core;

namespace NerdBotCommon.Importer.DataReaders
{
    public class MtgJsonReader : IMtgDataReader
    {
        private readonly IMtgDataMapper<MtgJsonCard, MtgJsonSet> mDataMapper;
        private string mFileName;
        private readonly ILoggingService mLoggingService;

        private bool mSkipTokens = true;

        public bool SkipTokens
        {
            set { this.mSkipTokens = value; }
        }

        public string FileName
        {
            get { return this.mFileName; }
            set
            {
                this.mFileName = value;
            }
        }

        public MtgJsonReader(
            string fileName,
            IMtgDataMapper<MtgJsonCard, MtgJsonSet> dataMapper,
            ILoggingService loggingService)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (dataMapper == null)
                throw new ArgumentNullException("dataMapper");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this.mFileName = fileName;
            this.mDataMapper = dataMapper;
            this.mLoggingService = loggingService;
        }

        public MtgJsonReader(
            IMtgDataMapper<MtgJsonCard, MtgJsonSet> dataMapper,
            ILoggingService loggingService)
        {
            if (dataMapper == null)
                throw new ArgumentNullException("dataMapper");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this.mDataMapper = dataMapper;
            this.mLoggingService = loggingService;
        }

        public Set ReadSet(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("data");

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

        public IEnumerable<Card> ReadCards(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("data");

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
                    if (!string.IsNullOrEmpty(cardData.Layout) && cardData.Layout.ToLower() == "token")
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
