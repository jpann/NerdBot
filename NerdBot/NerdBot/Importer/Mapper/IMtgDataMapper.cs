using NerdBot.Mtg;

namespace NerdBot.Importer.Mapper
{
    public interface IMtgDataMapper<TCard, TSet>
    {
        string ImageUrl { get; set; }
        string ImageHiResUrl { get; set; }

        Card GetCard(TCard source);
        Card GetCard(TCard source, string setName, string setCode);
        Set GetSet(TSet source);
    }
}