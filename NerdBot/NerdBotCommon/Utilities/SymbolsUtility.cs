using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NerdBotCommon.Utilities
{
    public static class SymbolsUtility
    {
        /// <summary>
        /// Replaces symbols in string with tags that will display those symbols via CSS.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string GetCostSymbols(string data)
        {
            string format = "<i class=\"ms {0} ms-cost ms-shadow\"></i>";

            Match[] matches = Regex.Matches(data, "(?<cost>{[a-zA-Z0-9/]+})")
                .Cast<Match>()
                .ToArray();

            List<string> costs = new List<string>();

            foreach (Match m in matches)
            {
                string cost = m.Value;

                cost = cost.Replace("{W}", "ms-w");
                cost = cost.Replace("{G}", "ms-g");
                cost = cost.Replace("{R}", "ms-r");
                cost = cost.Replace("{B}", "ms-b");
                cost = cost.Replace("{U}", "ms-u");
                cost = cost.Replace("{1}", "ms-1");
                cost = cost.Replace("{2}", "ms-2");
                cost = cost.Replace("{3}", "ms-3");
                cost = cost.Replace("{4}", "ms-4");
                cost = cost.Replace("{5}", "ms-5");
                cost = cost.Replace("{6}", "ms-6");
                cost = cost.Replace("{7}", "ms-7");
                cost = cost.Replace("{8}", "ms-8");
                cost = cost.Replace("{9}", "ms-9");
                cost = cost.Replace("{10}", "ms-10");
                cost = cost.Replace("{11}", "ms-11");
                cost = cost.Replace("{12}", "ms-12");
                cost = cost.Replace("{13}", "ms-13");
                cost = cost.Replace("{14}", "ms-14");
                cost = cost.Replace("{15}", "ms-15");
                cost = cost.Replace("{16}", "ms-16");
                cost = cost.Replace("{17}", "ms-17");
                cost = cost.Replace("{18}", "ms-18");
                cost = cost.Replace("{19}", "ms-19");
                cost = cost.Replace("{20}", "ms-20");
                cost = cost.Replace("{1/2}", "ms-1-2");
                cost = cost.Replace("{100}", "ms-100");
                cost = cost.Replace("{1000000}", "ms-1000000");
                cost = cost.Replace("{E}", "ms-e");
                cost = cost.Replace("{P}", "ms-p");
                cost = cost.Replace("{S}", "ms-s");
                cost = cost.Replace("{X}", "ms-x");
                cost = cost.Replace("{C}", "ms-c");
                cost = cost.Replace("{Y}", "ms-y");
                cost = cost.Replace("{Z}", "ms-z");

                // Phyrexian costs
                Match phyCostMatch = Regex.Match(cost, @"{(?<c>[GUBWR])/P}");
                if (phyCostMatch.Success)
                {
                    string color = phyCostMatch.Groups["c"].Value;

                    cost = string.Format("ms-{0}p ms-split", color.ToLower());
                }

                // Split costs
                //TODO Fix these
                Match splitCostMatch = Regex.Match(cost, @"{(?<a>[\d|\w])/(?<b>[\d|\w])}");
                if (splitCostMatch.Success)
                {
                    string colorA = splitCostMatch.Groups["a"].Value;
                    string colorB = splitCostMatch.Groups["b"].Value;

                    cost = string.Format("ms-{0}{1} ms-split", colorA.ToLower(), colorB.ToLower());
                }

                costs.Add(string.Format(format, cost));
            }

            string formattedCosts = string.Join(" ", costs);
            return formattedCosts;
        }

        public static string GetDescSymbols(string desc)
        {
            string format = "<i class=\"ms {0} ms-cost ms-shadow\"></i>";

            desc = desc.Replace("{W}", string.Format(format, "ms-w"));
            desc = desc.Replace("{R}", string.Format(format, "ms-r"));
            desc = desc.Replace("{G}", string.Format(format, "ms-g"));
            desc = desc.Replace("{B}", string.Format(format, "ms-b"));
            desc = desc.Replace("{U}", string.Format(format, "ms-u"));
            desc = desc.Replace("{1}", string.Format(format, "ms-1"));
            desc = desc.Replace("{2}", string.Format(format, "ms-2"));
            desc = desc.Replace("{3}", string.Format(format, "ms-3"));
            desc = desc.Replace("{4}", string.Format(format, "ms-4"));
            desc = desc.Replace("{5}", string.Format(format, "ms-5"));
            desc = desc.Replace("{6}", string.Format(format, "ms-6"));
            desc = desc.Replace("{7}", string.Format(format, "ms-7"));
            desc = desc.Replace("{8}", string.Format(format, "ms-8"));
            desc = desc.Replace("{9}", string.Format(format, "ms-9"));
            desc = desc.Replace("{10}", string.Format(format, "ms-10"));
            desc = desc.Replace("{11}", string.Format(format, "ms-11"));
            desc = desc.Replace("{12}", string.Format(format, "ms-12"));
            desc = desc.Replace("{13}", string.Format(format, "ms-13"));
            desc = desc.Replace("{14}", string.Format(format, "ms-14"));
            desc = desc.Replace("{15}", string.Format(format, "ms-15"));
            desc = desc.Replace("{16}", string.Format(format, "ms-16"));
            desc = desc.Replace("{17}", string.Format(format, "ms-17"));
            desc = desc.Replace("{18}", string.Format(format, "ms-18"));
            desc = desc.Replace("{19}", string.Format(format, "ms-19"));
            desc = desc.Replace("{20}", string.Format(format, "ms-20"));
            desc = desc.Replace("{100}", string.Format(format, "ms-100"));
            desc = desc.Replace("{1000000}", string.Format(format, "ms-1000000"));
            desc = desc.Replace("{X}", string.Format(format, "ms-x"));
            desc = desc.Replace("{C}", string.Format(format, "ms-c"));
            desc = desc.Replace("{Y}", string.Format(format, "ms-y"));
            desc = desc.Replace("{Z}", string.Format(format, "ms-z"));
            desc = desc.Replace("{S}", string.Format(format, "ms-s"));
            desc = desc.Replace("{E}", string.Format(format, "ms-e"));
            desc = desc.Replace("{P}", string.Format(format, "ms-p"));
            desc = desc.Replace("{T}", string.Format(format, "ms-tap"));
            desc = desc.Replace("{Q}", string.Format(format, "ms-untap"));

            // Phyrexian costs
            var phyRegex = new Regex(@"(?<cost>{(?<c>[GUBWR])/P})");
            var phyCostMatches = phyRegex.Matches(desc);

            if (phyCostMatches.Count > 0)
            {
                foreach (Match m in phyCostMatches)
                {
                    string color = m.Groups["c"].Value;
                    string matched = m.Groups["cost"].Value;

                    string cost = string.Format(format, string.Format("ms-{0}p ms-split", color.ToLower()));

                    desc = desc.Replace(matched, cost);
                }
            }

            // Split costs
            //TODO Fix these
            var splitRegex = new Regex(@"(?<cost>{(?<a>[\d|\w])/(?<b>[\d|\w])})");

            var splitMatches = splitRegex.Matches(desc);
            if (splitMatches.Count > 0)
            {
                foreach (Match m in splitMatches)
                {
                    string colorA = m.Groups["a"].Value;
                    string colorB = m.Groups["b"].Value;
                    string matched = m.Groups["cost"].Value;

                    string cost = string.Format(format, string.Format("ms-{0}{1} ms-split", colorA.ToLower(), colorB.ToLower()));

                    desc = desc.Replace(matched, cost);
                }
            }

            return desc;
        }
    }
}