﻿using System;
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


    public static class SteamProvider
    {
        public static IEnumerable<Game> ParseSteam(int[] subCategory, int count=25) //or an array of subcategories, i can do that, no problem
        {

            int Count = count; // how many items will be parsed, max is 25!
            if (Count > 25) { Count = 25; }

            //HtmlAgilityPack.HtmlDocument dokument = new HtmlDocument();
            //dokument.Load("C:\\testSteam.xml"); //for testing purposes


            string combinedCategories = subCategory[0].ToString();

            for (int i = 1; i < subCategory.Length; i++)
            {
                combinedCategories += subCategory[i].ToString();
            }

            String webSiteURL = "http://store.steampowered.com/search/?category1=998&tags=" + combinedCategories; //subCategory;//moglo bi i vise od 1 kategorije?

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

            int counter = 0;

            var itemList = new List<Game>();

            if (mainDiv.InnerHtml == "" || mainDiv.SelectNodes(".//a") == null)
            {
                return itemList;
            }

            //steam je blokiran na feru, PROBLEM! bar ako localhost radi!!!!!


            foreach (HtmlNode tag in mainDiv.SelectNodes(".//a")) //[@class='search_result_row ds_collapse_flag app_impression_tracked'] //this is generated afterwords by js?
            {

                if (counter >= Count) { break; }
                counter++;

                string tempGameURL = "";
                string tempGameID = "";

                HtmlAttribute gameURL = tag.Attributes["href"];
               // HtmlAttribute gameID = tag.Attributes["id"];
               
                if (gameURL != null) {
                    tempGameURL = gameURL.Value.Trim();
                    tempGameID = tempGameURL.Split(new string[] {"app/","/?"},StringSplitOptions.None)[1]; //trim i br?
                    //stringSeparators, StringSplitOptions.None)[1].Trim();
                }
               

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

                //  HtmlNode temp4 = link.SelectSingleNode("//p[@id='hover_desc']"); //dinamički ga nekako kasnije loada? js?

                float price = 0.0f;
                try
                {
                    if (tempPrice.Trim() != "Free to Play")
                    {
                        price = Convert.ToSingle(tempPrice.Split('&')[0],  new System.Globalization.CultureInfo("de-DE"));
                    }
                }
                catch (Exception ex)
                {
                    price = 0.0f;
                }
                 
                HtmlAgilityPack.HtmlWeb webPageDescription= new HtmlWeb();
                HtmlAgilityPack.HtmlDocument docDescription = webPageDescription.Load("http://store.steampowered.com/apphover/" + tempGameID);

                HtmlNode tempDescriptionNode = docDescription.DocumentNode.SelectSingleNode("//p[@id='hover_desc']");
              
                string tempDescription = tempDescriptionNode.InnerHtml;
                       //HtmlAttribute gameURL = tag.Attributes["href"];

                itemList.Add(new Game()
                {
                    Id = tempGameID,
                    Price = price,
                    Category = TestRepository.Categories.First(x=>x.Value.Id==subCategory[0]).Value,
                    Name = tempName,
                    LinkToTheStore = tempGameURL,
                    IconUrl = tempImageURL,
                    PriceCurrency = "€",
                    Store = Store.Steam,
                    Description = tempDescription,
                    Platform = "PC"
                });
            }

            return itemList;

        }


        internal static Item GetItemById(string itemId)
        {
            Item i = new Item { Name = "steam game", Description = "Desc", IconUrl = "",Id = "itemID", Price =9.99F, PriceCurrency = "€", Store = Store.Steam };
            return i;
        }
    }


}