using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GiftCaseBackend.Models
{

    /*
     * Example of response on GET /Profile/Vlatko
     * 
    <javaProfileModel>
        <billingplan>Prepaid</billingplan>
        <location>New York</location>
        <profileId>0</profileId>
        <username>Vlatko</username>
    </javaProfileModel> 
     * 
     * 
     */
    public class TelcoData
    {
        public string BillingPlan { get; set; }

        public string Location { get; set; }

        public string UserId { get; set; }

    }
}