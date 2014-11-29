using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net;
using System.Diagnostics;



namespace GiftCaseBackend.Models
{
    /* Obsolete
    public enum SteamTags
    {
        Action = 19,
        Indie = 492,
        Adventure = 21,
        RPG = 122,
        Simulation = 599,
        Casual = 597,
        Free2Play = 113,
        SinglePlayer = 4182,
        MMO = 128,
        MultiPlayer = 3859,
        Racing = 699,
        Sports = 701,
        Shooter = 1774,
        FPS = 1663,
        SciFi = 3942,
        Strategy = 9
    }*/

    public class SteamResult
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string GameURL { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }

        public SteamResult(string name, string image, string gameURL, string price, string description)
        {
            this.Name = name;
            this.Image = image;
            this.GameURL = gameURL;
            this.Price = price;
            this.Description = description; //cant parse it properly...YET!
        }

    }


    public static class SteamProvider
    {
        public static IEnumerable<Item> ParseSteam(int subCategory) //or an array of subcategories, i can do that, no problem
        {

            //HtmlAgilityPack.HtmlDocument dokument = new HtmlDocument();
            //dokument.Load("C:\\testSteam.xml"); //for testing purposes

            String webSiteURL = "http://store.steampowered.com/search/?tags=" + subCategory;//moglo bi i vise od 1 kategorije?

            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument dokument = web.Load(webSiteURL);
            //"http://store.steampowered.com/search/?term=#sort_by=_ASC&tags=9&page=1");


            //HtmlNode node = dokument.DocumentNode.SelectSingleNode("//body"); //or even "/body" because it's root

            //check out the info at:
            //http://www.w3schools.com/xpath/xpath_syntax.asp
            //http://articles.runtings.co.uk/2009/11/easily-extracting-links-from-snippet-of.html
            //http://stackoverflow.com/questions/846994/how-to-use-html-agility-pack


            HtmlNode mainDiv = dokument.DocumentNode.SelectSingleNode("//div[@id='search_result_container']");
            //Debug.WriteLine(node.InnerHtml);

            //List<SteamResult> popisIgrica = new List<SteamResult>();
            List<Item> itemList = new List<Item>();

            foreach (HtmlNode tag in mainDiv.SelectNodes(".//a")) //[@class='search_result_row ds_collapse_flag app_impression_tracked'] //this is generated afterwords by js?
            {
                string tempGameURL = "";
                HtmlAttribute gameURL = tag.Attributes["href"];
                if (gameURL != null) { tempGameURL = gameURL.Value.Trim(); }

                HtmlNode name = tag.SelectSingleNode(".//span[@class='title']");

                String tempName = "";

                if (name != null)
                {
                    tempName = name.InnerHtml.Trim();
                    Debug.WriteLine(tempName);
                }

                String tempPrice = "";

                HtmlNode priceNormal = tag.SelectSingleNode(".//div[@class='col search_price']");
                HtmlNode priceDiscounted = tag.SelectSingleNode(".//div[@class='col search_price discounted']"); //discounted has a different html
                HtmlNode priceFree = tag.SelectSingleNode(".//div[@class='col search_price ']"); //if it's free

                if (priceNormal != null)
                {
                    tempPrice = priceNormal.InnerHtml.Trim();
                }

                else if (priceNormal == null && priceDiscounted != null)
                {

                    priceNormal = priceDiscounted;
                    string[] stringSeparators = new string[] { "<br>", "<br/>" };
                    if (priceNormal != null)
                    {
                        tempPrice = priceNormal.InnerHtml.Split(stringSeparators, StringSplitOptions.None)[1].Trim();
                    }


                }

                else if (priceNormal == null && priceDiscounted == null && priceFree != null)
                {
                    priceNormal = priceFree;
                    if (priceNormal != null)
                    {
                        tempPrice = priceNormal.InnerHtml.Trim();
                    }

                }

                Debug.WriteLine(tempPrice);

                HtmlNode divPictureTag = tag.SelectSingleNode(".//div[@class='col search_capsule']");

                if (divPictureTag == null) { break; } //needs better break condition!

                string tempImageURL = "";

                HtmlNode imgTag = divPictureTag.SelectSingleNode(".//img[@src]");
                HtmlAttribute pictureURL = imgTag.Attributes["src"];

                if (pictureURL != null)
                {
                    tempImageURL = pictureURL.Value.Trim();
                    Debug.WriteLine(tempImageURL);
                }

                //  HtmlNode temp4 = link.SelectSingleNode("//p[@id='hover_desc']"); //dinamički ga nekako kasnije loada?

                float price = 0.0f;
                try
                {
                    if (tempPrice.Trim() != "Free to Play")
                    {
                        price = Convert.ToSingle(tempPrice.Split('&')[0]);
                    }
                }
                catch (Exception ex)
                {
                    price = 0.0f;
                }
                

                itemList.Add(new Item()
                {
                    Price = price,
                    Category = TestRepository.Categories.First(x=>x.Id==subCategory),
                    Name = tempName,
                    LinkToTheStore = tempGameURL,
                    IconUrl = tempImageURL,
                    PriceCurrency = "€"
                });//description fail x.x
            }

            return itemList;

        }

    }


}