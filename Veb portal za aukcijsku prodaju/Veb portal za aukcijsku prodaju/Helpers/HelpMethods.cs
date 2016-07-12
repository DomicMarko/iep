using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veb_portal_za_aukcijsku_prodaju.Models;

namespace Veb_portal_za_aukcijsku_prodaju.Helpers
{
    public class HelpMethods
    {
        public static void BidAuction(int auctionID, int userID, out string fullUserName, out string newPrice, out bool tokens, out double timeRemaining)
        {

            try
            {
                Aukcija aukcija = null;
                Korisnik korisnik = null;
                using (var context = new AukcijaEntities())
                {
                    aukcija = context.Aukcijas.Find(auctionID);
                    korisnik = context.Korisniks.Find(userID);
                }

                if ((aukcija != null) && (korisnik != null))
                {
                    if((korisnik.BrojTokena > 0) && (aukcija.Status == "OPEN"))
                    {
                        tokens = false;
                        double newPriceDouble = (double)aukcija.TrenutnaCena + 0.5;

                        double preostalo = ((DateTime)aukcija.VremeZatvaranja - DateTime.Now).TotalSeconds;

                        DateTime newDate = (DateTime)aukcija.VremeZatvaranja;
                        if(preostalo <= 10)
                        {
                            double increment = preostalo * (-1);
                            increment += 10; 
                            
                            newDate = newDate.AddSeconds(increment);                            
                        }

                        aukcija.VremeZatvaranja = newDate;
                        timeRemaining = aukcija.PreostaloVreme = ((DateTime)aukcija.VremeZatvaranja - DateTime.Now).TotalSeconds;                        

                        Bid newBid = new Bid()
                        {
                            PonCena = newPriceDouble,
                            Vreme = DateTime.Now,
                            KorisnikID = userID,
                            AukcijaID = auctionID
                        };

                        using (var context = new AukcijaEntities())
                        {                            

                            context.Entry(aukcija).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();

                            context.Bids.Add(newBid);
                            context.SaveChanges();

                            fullUserName = korisnik.Ime + " " + korisnik.Prezime;
                            newPrice = "" + newPriceDouble;
                        }
                    }
                    else
                    {
                        fullUserName = newPrice = null;

                        if(aukcija.Status != "OPEN")
                        {
                            tokens = false;
                            timeRemaining = -1;
                        }
                        else
                        {
                            tokens = true;
                            timeRemaining = 1;
                        }                        
                    }
                }
                else
                {
                    fullUserName = newPrice = null;
                    tokens = true;
                    timeRemaining = -1;
                }
            }
            catch (FormatException)
            {
                fullUserName = newPrice = null;
                tokens = true;
                timeRemaining = -1;
                Console.WriteLine("Something went wrong.");
            }
        }

        public static void ChangePrice(int? id, string newPrice)
        {
            try
            {
                double doubleValue = Convert.ToDouble(newPrice);
                Aukcija editAukcija;

                using (var context = new AukcijaEntities())
                {
                    editAukcija = context.Aukcijas.Find(id);
                }

                if (editAukcija != null)
                {
                    editAukcija.PocetnaCena = doubleValue;
                    editAukcija.TrenutnaCena = doubleValue;
                }

                using (var context = new AukcijaEntities())
                {
                    context.Entry(editAukcija).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert '{0}' to a Double.", newPrice);
            }
            catch (OverflowException)
            {
                Console.WriteLine("'{0}' is outside the range of a Double.", newPrice);
            }

            //return RedirectToAction("Index", "Admin", new { id = id });
        }

        public static void OpenAuction(int? id)
        {
            if (id == null)
            {
                return;
            }

            try
            {
                Aukcija editAukcija;

                using (var context = new AukcijaEntities())
                {
                    editAukcija = context.Aukcijas.Find(id);
                }

                if (editAukcija != null)
                {
                    DateTime startTime = DateTime.Now;
                    DateTime endTime = startTime.AddSeconds((int)editAukcija.Trajanje);

                    editAukcija.Status = "OPEN";
                    editAukcija.VremeOtvaranja = startTime;
                    editAukcija.VremeZatvaranja = endTime;
                }


                using (var context = new AukcijaEntities())
                {
                    context.Entry(editAukcija).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to delete auction");
            }

            // return RedirectToAction("Index", "Admin", new { id = id });
        }

        public static string AuctionOver(int id)
        {
            string result = "";
            Aukcija aukcija;

            using (var context = new AukcijaEntities())
            {
                aukcija = context.Aukcijas.Find(id);
            }


            if (aukcija != null)
            {
                if (!aukcija.Status.Equals("OPEN"))
                {
                    result = aukcija.Status;
                }
                else
                {
                    Nullable<int> lastBid = aukcija.BidID;

                    if (lastBid != null)
                        aukcija.Status = "SOLD";                        
                    else
                        aukcija.Status = "EXPIRED";

                    result = aukcija.Status;

                    using (var context = new AukcijaEntities())
                    {
                        context.Entry(aukcija).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        if(result == "SOLD")
                        {
                            Bid bid = context.Bids.Find(aukcija.BidID);
                            Korisnik korisnik = context.Korisniks.Find(bid.KorisnikID);

                            korisnik.Aukcijas.Add(aukcija);
                            context.SaveChanges();
                        }
                    }
                }

            }

            return result;
        }
    }
}