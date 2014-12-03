using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GiftCaseBackend.Models;

namespace GiftCaseBackend.Models
{
    public static class GiftRecommendationEngine
    {
        public static void CalculateAffinity(User user) //return user? Depends on where and how we save the user affinity.
        {
            int temp = new Random().Next(0, 4);
            if (temp == 0)
            {
                user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = 0;
                user.Affinity[TestRepository.ItemCategoryEnum.Book.ToString()] = 9;
                user.Affinity[TestRepository.ItemCategoryEnum.Game.ToString()] = 10;
                user.Affinity[TestRepository.ItemCategoryEnum.Movie.ToString()] = 0; 

            }
            else if (temp==1)
            {
                user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = 1;
                user.Affinity[TestRepository.ItemCategoryEnum.Book.ToString()] = 16;
                user.Affinity[TestRepository.ItemCategoryEnum.Game.ToString()] = 0;
                user.Affinity[TestRepository.ItemCategoryEnum.Movie.ToString()] = 1;
            }

            else if (temp == 2)
            {
                user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = 0;
                user.Affinity[TestRepository.ItemCategoryEnum.Book.ToString()] = 2;
                user.Affinity[TestRepository.ItemCategoryEnum.Game.ToString()] = 15;
                user.Affinity[TestRepository.ItemCategoryEnum.Movie.ToString()] = 0;
            }
            else if (temp == 3)
            {
                user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = 0;
                user.Affinity[TestRepository.ItemCategoryEnum.Book.ToString()] = 16;
                user.Affinity[TestRepository.ItemCategoryEnum.Game.ToString()] = 18;
                user.Affinity[TestRepository.ItemCategoryEnum.Movie.ToString()] = 1;
            }

        }
        public static  IEnumerable<Item> RecommendGifts(User user, int count, int? catID=-1)  //min-max price? The search should parse until it finds n results within the price range!
        {
            IEnumerable<Item> SteamGiftList = new List<Item>();
            IEnumerable<Item> AmazonGiftList = new List<Item>();
            IEnumerable<Item> ThirdGiftList = new List<Item>();

            List<Item> CombinedGiftList = new List<Item>();

            if (count > 25) //at least for now, because I don't want to parse multiple result pages
            {
                count = 25;
            }

            user.Id = "10152479696077544"; //Vlatko
            user.Id = "10152464438050382";//Ana

            FacebookProvider.UpdateAffinity(user);


            //somehow choose the 2 best categories and subcategories 
            int GameSubcategory = 122;
           

            const int subCatNum = 16;
            int[] choices = new int[subCatNum] { 3, 19, 492, 21, 122, 599, 597, 4182, 128, 3859, 699, 701, 1774, 1663, 3942, 9 };

            //subcats are like filters. AND filters, not OR filters, like I originally though!

            int numberOne = new Random().Next(0, subCatNum); //random subcat will be picked

            GameSubcategory = choices[numberOne];

          
           //all providers should take minPrice and maxPrice? Or should we filter it later?

            

            if (catID == -1) //search all categories -> engine selects the 2 categories that fit best
            {
                //fetch 1/3 of count from all providers
                SteamGiftList = SteamProvider.ParseSteam(new int[]{GameSubcategory},count / 3);
                //AmazonGiftList = AmazonProvider.SOMETHING!!!(subcategory,count/3);
                AmazonGiftList = TestRepository.Items.Where(x => x.Category.Id == 0 || x.Category.Id == 1 || x.Category.Id == 2).Take(count/3).ToList<Item>();
                //ThirdGiftList = ThirdProvider.SOMETHING(subcategory, count / 3);

                CombinedGiftList.AddRange(SteamGiftList);
                CombinedGiftList.AddRange(AmazonGiftList);
                CombinedGiftList.AddRange(ThirdGiftList);

               // return CombinedGiftList.Take(count); //or maybe even less? 
            }


            else //search only specific category 
            {
                if (catID == 0 || catID == 1 || catID == 2) //return dummy data for non-steam items
                { 
                        CombinedGiftList = TestRepository.Items.Where(x => x.Category.Id == catID).Take(count).ToList<Item>();
                }
                    /*
                else if (catID == 1)
                {

                }
                else if (catID == 2)
                {

                }
                     */
                else if (catID == 3)
                {
                    SteamGiftList = SteamProvider.ParseSteam(new int[]{GameSubcategory}, count);

                    CombinedGiftList.AddRange(SteamGiftList);
                }
            }

            /*
            //max affinity? if at least 2 above the threshold then split into 2, else do just 1. 
            //maybe even treshold for 3? Limit to 2 cuz it's easier?

            int FirstCatCount = 1;
            int SecondCatCount = 1;
            
            int numberOfCats = 1;

            if (numberOfCats == 1)
            {
                FirstCatCount = count;
            }

            else if (numberOfCats == 2)
            {

                user.Affinity.Values.Max();
                if (count == 1)
                {
                    FirstCatCount = 1;
                    SecondCatCount = 0;
                }
                else if (count == 2)
                {
                    FirstCatCount = 1;
                    SecondCatCount = 1;
                }

                else if (count > 2 && count <= 25)
                {
                    //split counter into 2, using div?

                    FirstCatCount = count / 2;
                    SecondCatCount = count / 2;

                    if ( (count % 2) == 0)
                    {
                        FirstCatCount += 1;
                    }
                }

                else //higher I guess?
                {

                }
            }

            //else return empty list?
            bool searchBooks = true;
            bool searchGames = true;
            bool searchAudio = false;
            bool searchMovies = false;


            //affinity for books is the sum of subcategory affinities like romance novels, sci-fi , fantasy....
            if (user.Affinity[ItemCategoryEnum.Book.ToString()] >= 10)
            {
                searchBooks = true;
            }

            if (user.Affinity[ItemCategoryEnum.Game.ToString()] >= 10)
            {
                searchGames = true;
            }

            if (user.Affinity[ItemCategoryEnum.Movie.ToString()] >= 10)
            {
                searchMovies = true;
            }

            if (user.Affinity[ItemCategoryEnum.Audio.ToString()] >= 10)
            {
                searchAudio = true;
            }
            
             IEnumerable<Item> tempGiftList = new List<Item>();

            //raspodijeli 3+2, ako count = 2, onda 1+1, ako 1 onda samo naj kategoriju

            if (searchBooks)
            {
                //gooo amazon, fetch books
            }

            if (searchAudio)
            {
                //goo amazon, fetch audio
            }

            if (searchGames)
            {
                //FirstCatCount or SecondCatCount count? Maybe parse the entire count, and take part of them later? all? 
               //tempGiftList = SteamProvider.ParseSteam(GameSubcategory,25);
            }

            if (searchMovies)
            {
                
            }
             //2 categories max?

            //what about subcategories?

          
            //naizmjenice uzimaj jednog, ili npr 2 od games, pa 1 od books, ovisno o omjeru!
           
            */


            //filter according to price
            //filter gifts already received

            return CombinedGiftList.Take(count);
            

        }
    }
}