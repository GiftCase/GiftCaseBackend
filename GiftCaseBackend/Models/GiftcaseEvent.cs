using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiftCaseBackend.Models
{
    public class GiftcaseEvent
    {
        public GiftcaseEventType Type { get; set; }
        public List<Contact> RelatedContacts { get; set; }
        public DateTime Date { get; set; }

        public string Details { get; set; }
    }

    public enum GiftcaseEventType
    {
        Anniversary, Birthday, Graduation,
    }
}