namespace NerdBotCommon.Web.Queries
{
    public class SearchQuery
    {
        public string SearchTerm { get; set; }
        public int Page { get; set; }

        public SearchQuery()
        {
            this.Page = 0;
        }
    }
}
