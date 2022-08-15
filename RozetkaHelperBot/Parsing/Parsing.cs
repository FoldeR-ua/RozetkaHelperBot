using System;
using HtmlAgilityPack;

namespace RozetkaHelperBot.Parsing
{
    class Parsing
    {
        public string Url { get; set; }

        public (HtmlNodeCollection title, HtmlNodeCollection price) ParsingNodesTP()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(this.Url);
            var titles = doc.DocumentNode.SelectNodes("//a[@class='goods-tile__heading ng-star-inserted']");
            var price = doc.DocumentNode.SelectNodes("//p[@class='ng-star-inserted']");

            return (titles, price);
        }
    }
}
