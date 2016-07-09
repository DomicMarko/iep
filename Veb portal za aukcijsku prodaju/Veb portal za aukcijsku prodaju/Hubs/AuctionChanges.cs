using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Veb_portal_za_aukcijsku_prodaju.Controllers;

namespace Veb_portal_za_aukcijsku_prodaju.Hubs
{
    public class AuctionChanges : Hub
    {
        public void SendBid(string auctionID, string userID)
        {
            string fullUserName = "";
            string newPrice = "";

            AccountController a = new AccountController();
            a.BidAuction(Int32.Parse(auctionID), Int32.Parse(userID), out fullUserName, out newPrice);


            // Call the updateLastBid method to update auction.
            Clients.All.updateLastBid(auctionID, fullUserName, newPrice);
        }
/*
        public void Hello()
        {
            Clients.All.hello();
        }
 * */
    }
}
