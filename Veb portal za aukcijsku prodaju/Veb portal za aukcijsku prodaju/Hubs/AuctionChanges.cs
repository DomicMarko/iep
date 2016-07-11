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
            Clients.All.updateLastBidHome(auctionID, fullUserName, newPrice);
            Clients.All.updateLastBidAuction(auctionID, fullUserName, newPrice);
        }

        public void ChangeStartPrice(string auctionID, string newPrice)
        {

            AuctionController a = new AuctionController();
            a.ChangePrice(Int32.Parse(auctionID), newPrice);            

            Clients.All.changeStartPriceAdmin(auctionID, newPrice);
        }

        public void ActivateAuction(string auctionID, double startCalc)
        {

            AuctionController a = new AuctionController();
            a.OpenAuction(Int32.Parse(auctionID));

            Clients.All.auctionOpenedHome(auctionID, startCalc);
            Clients.All.auctionOpenedAuction(auctionID);            
        }

        public void ChangeAuctionStatusOver(string auctionID)
        {

            Clients.All.auctionStatusChangedOver(auctionID, newStatus);
        }
/*
        public void Hello()
        {
            Clients.All.hello();
        }
 * */
    }
}
