using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class AWSSNSController : PrimaryController
    {

        public ActionResult Index()
        {
            return View();
        }
            // GET: AWSSNS
            [HttpPost]
            public ActionResult Index(string PhoneNumber,string TextMessage)
        {
            //var options = new CredentialProfileOptions
            //{
            //    AccessKey = "AKIAJOXPDJ5YJHKWDZVA",
            //    SecretKey = "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"
            //};
            //var profile = new Amazon.Runtime.CredentialManagement.CredentialProfile("basic_profile", options);
            //profile.Region = RegionEndpoint.EUWest1;
            //var netSDKFile = new NetSDKCredentialsFile();
            //netSDKFile.RegisterProfile(profile);

            var snsClient = new AmazonSimpleNotificationServiceClient("AKIAJOXPDJ5YJHKWDZVA", "8zTizwOajlygC0ZNMosbr2x5g7OyJ/Srqh3+Y2xb");

            String message = TextMessage;//"SOS SMS message";
            String phoneNumber = PhoneNumber;//"+919871073209";//"+61450280180";
            //byte[] data = UTF8Encoding.UTF8.GetBytes("mySenderID");
            Dictionary<System.String, MessageAttributeValue> smsAttributes = new Dictionary<string, MessageAttributeValue>();
            snsClient.Publish(new PublishRequest()
            {
                Message = message,
                PhoneNumber = phoneNumber,
                MessageAttributes = smsAttributes
            });

            //Console.ReadLine();
            //var snsClient = new AmazonSimpleNotificationServiceClient();

            //var request = new ConfirmSubscriptionRequest
            //{
            //    TopicArn = "arn:aws:sns:us-east-1:80398EXAMPLE:CodingTestResults",
            //    Token = "2336412f37fb687f5d51e6e241d638b059833563d4ff1b6f50a3be00e3a" +
            //    "ff3a5f486f64ab082b19d3b9a6e569ea3f6acb10d944314fc3af72ebc36085519" +
            //    "3a02f5a8631552643b8089c751cb8343d581231fb631f34783e30fd2d959dd5bb" +
            //    "ea7b11ef09dbd06023af5de4d390d53a10dc9652c01983b028206a1b3e00EXAMPLE"
            //};

            //snsClient.ConfirmSubscription(request);
            return RedirectToAction("Index");
        }
    }
}