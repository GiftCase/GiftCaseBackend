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
        /// http://localhost:22467/api/Gifts/SuggestGift?userName=ana
        /// </summary>
        /// <param name="username">Name of the friend to whom to recommend a gift for</param>
        /// <param name="count"></param>
        /// <returns>List of gift recommendations</returns>
        [HttpGet]
        public IEnumerable<Item> SuggestGift(string username, int count=3)
        {
            return TestRepository.Gifts.Take(count);
        }

        /// <summary>
        /// Recommends some gifts for a friend from specified gift category
        /// URL example:
        /// http://localhost:22467/api/Gifts/SuggestGift?userName=ana&category=Book
        /// </summary>
        /// <param name="username">Name of the friend to whom to recommend a gift for</param>
        /// <param name="category">Category of gift</param>
        /// <param name="count"></param>
        /// <returns>List of gift recommendations</returns>
        public IEnumerable<Item> SuggestGift(string username, ItemCategory category, int count=3)
        {
            return TestRepository.Gifts.Where(x => x.Category == category).Take(count);
        }
    }
}
