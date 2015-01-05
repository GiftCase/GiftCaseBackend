using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using Amazon.PAAPI.WCF;
using GiftCaseBackend.Amazon;
using Newtonsoft.Json;

namespace GiftCaseBackend.Models
{

    /****************************************************************
     *              Usage Instructions
     ****************************************************************
     * Details about Item Search: http://docs.aws.amazon.com/AWSECommerceService/latest/DG/ItemSearch.html
     * Details about Amazon response elements: http://docs.aws.amazon.com/AWSECommerceService/latest/DG/CHAP_response_elements.html
     * 
     * To search for an item on amazon you need to supply the following:
     * SearchIndex - string, name of the category you are searching
     * BrowseNode - string, id of the node you are searching
     * 
     * Optional:
     * Sort - string, how the data will be sorted (default value is relevance)
     * ResponseGroup - string value, it defines which data Amazon will send back as a response (default value is "Small")
     * MinimumPrice - filter by price, serialized float into string
     * MaximumPrice - filter by price, serialized float into string
     * Title - search by string title
     ****************************************************************
     *          SearchIndex & BrowseNode
     ****************************************************************
     * List of nodes: http://docs.aws.amazon.com/AWSECommerceService/latest/DG/BrowseNodeIDs.html
     * Relevant categories and nodes for GiftCase:
     * 
     * SearchIndex:         US Node         UK Node
     * ----------------------------------------------
     * Books                1000            1025612
     * Video                130             283926
     * Music                301668          505510
     * VideoGames           468642          1025616
     * 
     ***************************************************************
     *                  Sort
     ***************************************************************
     *Valid sort values depend on the item category.
     *List of sort values: http://docs.aws.amazon.com/AWSECommerceService/latest/DG/USSortValuesArticle.html
     * ---------------------------------------
     * Sort values for Books:
     * ---------------------------------------
     * relevancerank	Items ranked according to the following criteria: how often the keyword 
     *                  appears in the description, where the keyword appears (for example, the ranking 
     *                  is higher when keywords are found in titles), how closely they occur in descriptions 
     *                  (if there are multiple keywords), and how often customers purchased the products they found using the keyword.
     * salesrank	    Bestselling first
     * reviewrank	    Average customer review: high to low
     * pricerank	    Price: low to high
     * inverse-pricerank	Price: high to low
     * daterank	        Publication date: newer to older
     * titlerank	    Alphabetical: A to Z
     * -titlerank	    Alphabetical: Z to A
     * 
     * ---------------------------------------
     * Sort values for Video:
     * ---------------------------------------
     * relevancerank	Items ranked according to the following criteria: how often the keyword appears in the description, 
     *                  where the keyword appears (for example, the ranking is higher when keywords are found in titles), 
     *                  how closely they occur in descriptions (if there are multiple keywords), and how often customers purchased 
     *                  the products they found using the keyword.
     * salesrank	    Bestselling
     * price	        Price: low to high
     * -price	        Price: high to low
     * titlerank	    Alphabetical: A to Z
     * -video-release-date	Release date: newer to older
     * -releasedate	    Release date: newer to older
     * 
     * ---------------------------------------
     * Sort values for Music:
     * ---------------------------------------
     * psrank	        Bestseller ranking taking into consideration projected sales.The lower the value, the better the sales.
     * salesrank	    Bestselling first
     * price	        Price: low to high
     * -price	        Price: high to low
     * titlerank	    Alphabetical: A to Z
     * -titlerank	    Alphabetical: Z to A
     * artistrank	    Artist name: A to Z
     * orig-rel-date	Original release date of the item listed from newer to older. See release-date, which sorts by the latest release date.
     * release-date	    Sorts by the latest release date from newer to older. See orig-rel-date, which sorts by the original release date.
     * releasedate	    Release date: most recent to oldest
     * -releasedate	    Release date: oldest to most recent
     * relevancerank	Items ranked according to the following criteria: how often the keyword appears in the description, 
     *                  where the keyword appears (for example, the ranking is higher when keywords are found in titles), 
     *                  how closely they occur in descriptions (if there are multiple keywords), and how often customers 
     *                  purchased the products they found using the keyword.
     *                  
     * ---------------------------------------
     * Sort values for VideoGames:
     * ---------------------------------------
     * pmrank	        Featured items
     * salesrank	    Bestselling
     * price	        Price: low to high
     * -price	        Price: high to low
     * titlerank	    Alphabetical: A to Z
     * 
     ****************************************************************
     *                  Response Groups
     ***************************************************************
     *Valid values for ResponseGroup: [
        'Tags', 'Help', 'ListMinimum', 'VariationSummary', 'VariationMatrix',
        'TransactionDetails', 'VariationMinimum', 'VariationImages',
        'PartBrandBinsSummary', 'CustomerFull', 'CartNewReleases',
        'ItemIds', 'SalesRank', 'TagsSummary', 'Fitments',
        'Subjects', 'Medium', 'ListmaniaLists',
        'PartBrowseNodeBinsSummary', 'TopSellers', 'Request',
        'HasPartCompatibility', 'PromotionDetails', 'ListFull',
        'Small', 'Seller', 'OfferFull', 'Accessories',
        'VehicleMakes', 'MerchantItemAttributes', 'TaggedItems',
        'VehicleParts', 'BrowseNodeInfo', 'ItemAttributes',
        'PromotionalTag', 'VehicleOptions', 'ListItems', 'Offers',
        'TaggedGuides', 'NewReleases', 'VehiclePartFit',
        'OfferSummary', 'VariationOffers', 'CartSimilarities',
        'Reviews', 'ShippingCharges', 'ShippingOptions', 'EditorialReview',
        'CustomerInfo', 'PromotionSummary', 'BrowseNodes',
        'PartnerTransactionDetails', 'VehicleYears', 'SearchBins',
        'VehicleTrims', 'Similarities', 'AlternateVersions',
        'SearchInside', 'CustomerReviews', 'SellerListing',
        'OfferListings', 'Cart', 'TaggedListmaniaLists',
        'VehicleModels', 'ListInfo', 'Large', 'CustomerLists',
        'Tracks', 'CartTopSellers', 'Images', 'Variations',
        'RelatedItems','Collections'
         ].
     */

    public static class AmazonProvider
    {
        private static readonly string AccessKeyId;
        private static readonly string SecretKey;

        private static dynamic Keys;

        private const string EndpointUrl = "https://webservices.amazon.com/onca/soap?Service=AWSECommerceService";

        private static AWSECommerceServicePortTypeClient Client;

        #region private Methods
        static AmazonProvider()
        {
            string text = BaaS.GetKeys("Amazon");
            Keys = JsonConvert.DeserializeObject(text);
            AccessKeyId = Keys.Public;
            SecretKey = Keys.Private;

            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;
            // create a WCF Amazon ECS client
            Client = new AWSECommerceServicePortTypeClient(binding, new EndpointAddress(EndpointUrl));
            Client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(AccessKeyId, SecretKey));
           
        }

        private static ItemSearch GetSearch(ItemSearchRequest request)
        {
            // create the search
            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new[] { request };
            itemSearch.AWSAccessKeyId = AccessKeyId;
            itemSearch.AssociateTag = "";
            return itemSearch;
        }

        private static ItemSearchRequest GetSearchRequest(float minPrice, float maxPrice)
        {
            // prepare an ItemSearch request
            ItemSearchRequest request = new ItemSearchRequest();
            // sort the results so best selling books are first results we get
            request.Sort = "salesrank";
            // fetch basic properties, prices and small image link
            request.ResponseGroup = new string[] { "Small", "OfferSummary", "Images", "EditorialReview", "ItemAttributes" };
            // we also want "EditorialReview" to get something like a description for the item, however, if we add that
            // the response size exceeds the limit. We will have to make a separate query or something.

            // set minimum and maximum price
            if (minPrice > 0)
                request.MinimumPrice = minPrice.ToString(CultureInfo.InvariantCulture);
            if (maxPrice > minPrice && maxPrice > 0)
                request.MaximumPrice = maxPrice.ToString(CultureInfo.InvariantCulture);
            return request;
        }

        private static Book GetBook(Amazon.Item item)
        {
            // setting of properties that should never cause an exception
            Book book = new Book()
            {
                Name = item.ItemAttributes.Title,
                Category = TestRepository.Categories[TestRepository.ItemCategoryEnum.Book.ToString()],
                Id = item.ASIN,
                LinkToTheStore = item.DetailPageURL.Replace("null", ""),
                PreviousPrice = 0,
                Store = Store.Amazon,
            };

            // setting of properties that might cause an exception in some rare cases
            try{book.Description = item.EditorialReviews[0].Content;}catch (Exception) { }
            try{book.Author = item.ItemAttributes.Author[0];}catch (Exception) { }
            try{book.IconUrl = item.SmallImage.URL;}catch (Exception) { }
            try{book.PriceCurrency = (item.OfferSummary.LowestNewPrice.CurrencyCode == "USD") ? "$" : item.OfferSummary.LowestNewPrice.CurrencyCode;}catch (Exception) { }

            float price = 0;
            float.TryParse(item.OfferSummary.LowestNewPrice.FormattedPrice.Substring(1), out price);
            book.Price = price;

            return book;
        }

        private static Video GetVideo(Amazon.Item item)
        {
            var video = new Video()
            {
                Name = item.ItemAttributes.Title,
                Category = TestRepository.Categories[TestRepository.ItemCategoryEnum.Video.ToString()],
                Id = item.ASIN,
                LinkToTheStore = item.DetailPageURL.Replace("null", ""),
                PreviousPrice = 0,
                Store = Store.Amazon,
            };

            try {video.Description = item.EditorialReviews[0].Content; }catch(Exception e){}
            try {video.Director = item.ItemAttributes.Director[0]; }catch(Exception e){}
            try {video.IconUrl = item.SmallImage.URL; }catch(Exception e){}
            try {video.PriceCurrency = (item.OfferSummary.LowestNewPrice.CurrencyCode == "USD") ? "$" : item.OfferSummary.LowestNewPrice.CurrencyCode; }catch(Exception e){}

            float price = 0;
            float.TryParse(item.OfferSummary.LowestNewPrice.FormattedPrice.Substring(1), out price);
            video.Price = price;

            return video;
        }

        private static Music GetMusic(Amazon.Item item)
        {
            var music = new Music()
            {
                Name = item.ItemAttributes.Title,
                Category = TestRepository.Categories[TestRepository.ItemCategoryEnum.Audio.ToString()],
                Id = item.ASIN,
                LinkToTheStore = item.DetailPageURL.Replace("null", ""),
                PreviousPrice = 0,
                Store = Store.Amazon,
            };

            try {music.Description = item.EditorialReviews[0].Content; }catch(Exception e){}
            try {music.Artist = item.ItemAttributes.Artist[0]; }catch(Exception e){}
            try {music.IconUrl = item.SmallImage.URL; }catch(Exception e){}
            try {music.PriceCurrency = (item.OfferSummary.LowestNewPrice.CurrencyCode == "USD") ? "$" : item.OfferSummary.LowestNewPrice.CurrencyCode; }catch(Exception e){}

            float price = 0;
            float.TryParse(item.OfferSummary.LowestNewPrice.FormattedPrice.Substring(1), out price);
            music.Price = price;

            return music;
        }

        private static Game GetGame(Amazon.Item item)
        {
            var game = new Game()
            {
                Name = item.ItemAttributes.Title,
                Category = TestRepository.Categories[TestRepository.ItemCategoryEnum.Game.ToString()],
                Id = item.ASIN,
                LinkToTheStore = item.DetailPageURL.Replace("null", ""),
                PreviousPrice = 0,
                Store = Store.Amazon
            };
            try{game.Description = item.EditorialReviews[0].Content;}catch (Exception) { }
            try{game.IconUrl = item.SmallImage.URL;}catch (Exception) { }
            try{game.PriceCurrency = (item.OfferSummary.LowestNewPrice.CurrencyCode == "USD") ? "$" : item.OfferSummary.LowestNewPrice.CurrencyCode;}catch (Exception) { }

            try{game.Platform = item.ItemAttributes.OperatingSystem;}catch (Exception) { }
            if (game.Platform == null)
            {
                try{game.Platform = item.ItemAttributes.HardwarePlatform;}catch (Exception) { }
            }

            float price = 0;
            float.TryParse(item.OfferSummary.LowestNewPrice.FormattedPrice.Substring(1), out price);
            game.Price = price;

            return game;
        }
        #endregion

        #region Browse Methods
        public static IEnumerable<Book> BrowseBooks(float minPrice, float maxPrice)
        {
            var request = GetSearchRequest(minPrice, maxPrice);
            
            // Browse books category and US books category node
            request.SearchIndex = "Books";
            request.BrowseNode  = "1000";
           
            // issue the ItemSearch request
            ItemSearchResponse response = Client.ItemSearch(GetSearch(request));

            // convert the results into internal Book format
            var convertedItems = new List<Book>();
            var items = response.Items[0].Item;
            foreach (var item in items)
            {
                var book = GetBook(item);
                convertedItems.Add(book);
            }

            return convertedItems;
        }

        public static IEnumerable<Video> BrowseVideo(float minPrice, float maxPrice)
        {
            var request = GetSearchRequest(minPrice, maxPrice);

            // Browse books category and US books category node
            request.SearchIndex = "Video";
            request.BrowseNode = "130";

            // issue the ItemSearch request
            ItemSearchResponse response = Client.ItemSearch(GetSearch(request));

            // convert the results into internal Book format
            var convertedItems = new List<Video>();
            var items = response.Items[0].Item;
            foreach (var item in items)
            {
                var video = GetVideo(item);

                convertedItems.Add(video);
            }

            return convertedItems;
        }

        public static IEnumerable<Music> BrowseMusic(float minPrice, float maxPrice)
        {
            var request = GetSearchRequest(minPrice, maxPrice);

            // Browse books category and US books category node
            request.SearchIndex = "Music";
            request.BrowseNode = "301668";


            // issue the ItemSearch request
            ItemSearchResponse response = Client.ItemSearch(GetSearch(request));

            // convert the results into internal Book format
            var convertedItems = new List<Music>();
            var items = response.Items[0].Item;
            foreach (var item in items)
            {
                var music = GetMusic(item);

                convertedItems.Add(music);
            }

            return convertedItems;
        }

        public static IEnumerable<Game> BrowseVideoGames(float minPrice, float maxPrice)
        {
            var request = GetSearchRequest(minPrice, maxPrice);

            // Browse books category and US books category node
            request.SearchIndex = "VideoGames";
            request.BrowseNode = "468642";

            // issue the ItemSearch request
            ItemSearchResponse response = Client.ItemSearch(GetSearch(request));

            // convert the results into internal Book format
            var convertedItems = new List<Game>();
            var items = response.Items[0].Item;
            foreach (var item in items)
            {
                var game = GetGame(item);

                // try to weed out non games
                if(game.Platform!=null)
                    convertedItems.Add(game);
            }

            return convertedItems;
        }
        #endregion

        public static IEnumerable<Item> FindRelatedItems(string[] itemIds)
        {
            SimilarityLookupRequest request = new SimilarityLookupRequest();
            request.ItemId = itemIds;
            request.ResponseGroup = new string[] { "Small", "OfferSummary", "Images", "EditorialReview", "ItemAttributes" };

            SimilarityLookup lookup = new SimilarityLookup();
            lookup.Request = new[] { request };
            lookup.AWSAccessKeyId = AccessKeyId;
            lookup.AssociateTag = "";

            var response = Client.SimilarityLookup(lookup);

            // convert the results into internal Book format
            var convertedItems = new List<Item>();
            var items = response.Items[0].Item;

            foreach (var item in items)
            {
                Item convertedItem = null;

                // find out which type the item is

                // how do I even find out which type the item is?
                // I should probably use browseNodeids to find out, but I'll do that later
                // for now, this'll have to do:

                if (item.ItemAttributes.Author!=null)
                    convertedItem = GetBook(item);
                else if (item.ItemAttributes.Artist!=null)
                    convertedItem = GetMusic(item);
                else if(item.ItemAttributes.Director!=null)
                    convertedItem = GetVideo(item);
                else 
                    convertedItem = GetGame(item);


                if(convertedItem!=null && !(convertedItem is Game && ((Game)convertedItem).Platform==null))
                    convertedItems.Add(convertedItem);
            }
            return convertedItems;
        }

        /// <summary>
        /// Find a specific amazon item by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static Item GetItemByName(string name, ItemCategory category)
        {
            return null;
        }
    }
}