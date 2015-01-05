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
                user.Affinity[TestRepository.ItemCategoryEnum.Video.ToString()] = 0; 

            }
            else if (temp==1)
            {
                user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = 1;
                user.Affinity[TestRepository.ItemCategoryEnum.Book.ToString()] = 16;
                user.Affinity[TestRepository.ItemCategoryEnum.Game.ToString()] = 0;
                user.Affinity[TestRepository.ItemCategoryEnum.Video.ToString()] = 1;
            }

            else if (temp == 2)
            {
                user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = 0;
                user.Affinity[TestRepository.ItemCategoryEnum.Book.ToString()] = 2;
                user.Affinity[TestRepository.ItemCategoryEnum.Game.ToString()] = 15;
                user.Affinity[TestRepository.ItemCategoryEnum.Video.ToString()] = 0;
            }
            else if (temp == 3)
            {
                user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = 0;
                user.Affinity[TestRepository.ItemCategoryEnum.Book.ToString()] = 16;
                user.Affinity[TestRepository.ItemCategoryEnum.Game.ToString()] = 18;
                user.Affinity[TestRepository.ItemCategoryEnum.Video.ToString()] = 1;
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

           // user.Id = "10152479696077544"; //Vlatko
            //user.Id = "10152464438050382";//Ana

            FacebookProvider.UpdateAffinity(user);
            FacebookProvider.FetchEvents(user);
            //FacebookProvider.FetchGiftCaseFriends(user);
            //FacebookProvider.FetchInviteableFriends(user);
           // FacebookProvider.FetchExtendedToken(user, "CAAMewqUUav0BAHdiLkH4xE210I75ZC6kCSuuaLqXcQnaOkjq1ocSpxjMoZB947ff7orVzELGeJez19qNJjh0FZCNWR136yGWh6jiUK0GkgKCeFhEBGj9NC8FaBcFI2kpf3QUmOoXjWDDlXFChACTZAZBovo5ZCu6N2hMlsgCJGxGtxVdWRRy8NMW6YEZC4GTV0kDGR62PgikpEkaxLx5ZCbK");

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

                KeyValuePair<string, int>[] values = user.Affinity.OrderByDescending(x => x.Value).Select(x => x).ToArray();
                // var keys = user.Affinity.Where(x => someValues.Contains(x.Value)).Select(x => x.Key);
                int[] countSplit = { 0, 0 };

                /*
                 br1 = Math.Ceiling ( (double)(mojAff / sumaAff )* count) //zaokruzi na visi? tipa 10 rastavi na 6.6 i 3.3 -> 7 i 3, ovisno o omjerima? možda ipak bolje 50/50 ?
                 br2 = Math.Floor ( (double)(mojAff / sumaAff )* count)    // drugi zaokruzi na nizi
                */

              //  NOPE !!! countSplit[0] = (values[0].Value / 2) + (values[0].Value % 2); //the primary one will get +1 more if count is an odd number
               // countSplit[1] = values[1].Value / 2;

                countSplit[0] = (count / 2) + (count % 2);
                countSplit[1] = count / 2;

                //what if all affinities are zero????

                for (int i = 0; i < 2; i++)
                {
                    if (values[i].Key == TestRepository.ItemCategoryEnum.Audio.ToString())
                    {
                        // fetch something
                    }

                    else if (values[i].Key == TestRepository.ItemCategoryEnum.Book.ToString())
                    {
                        //AmazonGiftList = AmazonProvider.SOMETHING!!!(subcategory,count/3);
                       // AmazonGiftList = TestRepository.Items.Where(x => x.Category.Id == 0 || x.Category.Id == 1 || x.Category.Id == 2).Take(count / 3).ToList<Item>();
                    }

                    else if (values[i].Key == TestRepository.ItemCategoryEnum.Game.ToString())
                    {
                        SteamGiftList = SteamProvider.ParseSteam(new int[] { GameSubcategory }, countSplit[i]);
                        CombinedGiftList.AddRange(SteamGiftList.Take(countSplit[i]));
                    }

                    else if (values[i].Key == TestRepository.ItemCategoryEnum.Video.ToString())
                    {
                       // ThirdGiftList = ThirdProvider.SOMETHING(subcategory, count / 3); 
                        AmazonGiftList =  AmazonProvider.BrowseVideo(5, 50);
                        CombinedGiftList.AddRange(AmazonGiftList.Take(countSplit[i]));
                    }
                }

              //  CombinedGiftList.AddRange(SteamGiftList);
              //  CombinedGiftList.AddRange(AmazonGiftList);
              //  CombinedGiftList.AddRange(ThirdGiftList);

               // return CombinedGiftList.Take(count); //or maybe even less? 
            }


            else //search only specific category 
            {
                /* 
                //used for dummy data
                if (catID == 0 || catID == 1 || catID == 2) //return dummy data for non-steam items
                { 
                        CombinedGiftList = TestRepository.Items.Where(x => x.Category.Id == catID).Take(count).ToList<Item>();
                }
                else
                */

                if (catID == 1)
                {
                    AmazonGiftList =  AmazonProvider.BrowseBooks(2, 50);
                    CombinedGiftList.AddRange(AmazonGiftList.Take(count));
                }
                else if (catID == 2)
                { 
                    AmazonGiftList = AmazonProvider.BrowseVideo(5, 50);
                    CombinedGiftList.AddRange(AmazonGiftList.Take(count));
                }
                 
                else if (catID == 3) //replace 3 with enums
                {
                    SteamGiftList = SteamProvider.ParseSteam(new int[]{GameSubcategory}, count);

                    CombinedGiftList.AddRange(SteamGiftList);
                }

                else if (catID == 4) //itnues music or amazon music?!!!!!
                {
                    AmazonGiftList = AmazonProvider.BrowseMusic(5, 50);
                    CombinedGiftList.AddRange(AmazonGiftList.Take(count));
                }
            }

         


            //filter according to price
            //filter gifts already received
            Random rnd = new Random();
            List<Item> CombinedGiftList2 = CombinedGiftList.OrderBy(a => rnd.Next()).ToList();  // or .OrderBy(a => Guid.NewGuid()).ToList();
            return CombinedGiftList2.Take(count);

           // return CombinedGiftList.Take(count);
            

        }
    }
}