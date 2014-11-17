using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GiftCaseBackend.Models;

namespace GiftCaseBackend.Controllers
{
    public class GiftsController : ApiController
    {
        /// <summary>
        /// Recommends some gifts
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/SuggestGift?userName=ana
        /// </summary>
        /// <param name="username">Name of the friend to whom to recommend a gift for</param>
        /// <param name="count"></param>
        /// <returns>List of gift recommendations</returns>
        [HttpGet]
        public IEnumerable<Item> SuggestGift(string username, int count=3)
        {
            return TestRepository.Items.Take(count);
        }

        /// <summary>
        /// Recommends some gifts for a friend from specified gift category
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/SuggestGift?userName=ana&category=1
        /// </summary>
        /// <param name="username">Name of the friend to whom to recommend a gift for</param>
        /// <param name="category">Id of category of gift</param>
        /// <param name="count"></param>
        /// <returns>List of gift recommendations</returns>
        [HttpGet]
        public IEnumerable<Item> SuggestGift(string username, int category, int count=3)
        {
            return TestRepository.Items.Where(x => x.Category.Id == category).Take(count);
        }

        /// <summary>
        /// Recommends some gifts for a friend from specified gift category
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/SuggestGift?userName=ana&category=Book
        /// </summary>
        /// <param name="username">Name of the friend to whom to recommend a gift for</param>
        /// <param name="category">category name of gift</param>
        /// <param name="count"></param>
        /// <returns>List of gift recommendations</returns>
        [HttpGet] 
        public IEnumerable<Item> SuggestGift(string username, string category, int count = 3)
        {
            return TestRepository.Items.Where(x => x.Category.Name == category).Take(count);
        }
    }
}
