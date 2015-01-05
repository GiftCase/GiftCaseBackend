using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http.Routing;

namespace GiftCaseBackend.Models.ContentProviders
{
    /// <summary>
    /// URL: https://www.apple.com/itunes/affiliates/resources/documentation/itunes-store-web-service-search-api.html
    /// media: music, movie, audiobook, ebook, tvShow
    /// entity: album, movie, audiobook, ebook, tvSeason
    /// limit: number of search results
    /// 
    /// </summary>
    public static class iTunesProvider
    {
        private const string BasseUrl = "https://itunes.apple.com/";

        private static Music PopulateWithResults(dynamic item)
        {
            //todo:
            var music = new Music();
            music.Artist = item.artistName;
            music.ArtistId = item.artistId;
            music.Category = TestRepository.Categories[TestRepository.ItemCategoryEnum.Audio.ToString()];
            music.Description = "Fantastic new album!";
            music.IconUrl = item.artworkUrl100;
            music.Id = item.collectionId;
            music.LinkToTheStore = item.viewURL;
            music.Name = item.collectionName;
            music.Price = item.collectionPrice;
            music.Store = Store.iTunes;
            return music;
        }

        private static dynamic GetJsonResponse(string requestUrl)
        {
            var request = WebRequest.CreateHttp(requestUrl);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            dynamic JSONReponse = JsonConvert.DeserializeObject(result); //if no events, this is null!
            return JSONReponse;
        }
        
        public static List<Music> BrowseMusic(int count = 5)
        {
            var requestUrl = BasseUrl+"search?"+
                "term=*&"+
            // "5&attribute=ratingIndex&" + // search by rating, doesn't work
                "media=music&entity=album&" + // search music albums
                "limit="+count;


            var JSONReponse = GetJsonResponse(requestUrl);


            var list = new List<Music>(); //JSONReponse.resultCount
            foreach (var item in JSONReponse.results)
            {
                list.Add(PopulateWithResults(item));
            }
            return list;
        }

        /// <summary>
        /// Id can be an id of the album, in which case it returns just that one album
        /// It can also be an artist Id, in which case it returns all albums of that artist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<Music> GetMusicById(string id)
        {
            var requestUrl = BasseUrl +
                "lookup?id="+id;


            var JSONReponse = GetJsonResponse(requestUrl);
            var list = new List<Music>(); 
            foreach (var item in JSONReponse.results)
            {
                list.Add(PopulateWithResults(item));
            }
            return list;
        }

        public static Music SearchByAlbumName(string albumName)
        {
            var requestUrl = BasseUrl+"search?"+
                "term="+HttpUtility.UrlEncode(albumName)+
                "&attribute=albumTerm&" + // search by album name
                "media=music&entity=album&" + // search music albums
                "limit=1";


            var JSONReponse = GetJsonResponse(requestUrl);
            var item = JSONReponse.results[0];

            return null;
        }

    }
}