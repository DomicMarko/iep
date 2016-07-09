﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veb_portal_za_aukcijsku_prodaju.Models;
using Veb_portal_za_aukcijsku_prodaju.Models.Authentication;
using System.Data;
using System.Data.Entity;
using System.Net;
using PagedList;

namespace Veb_portal_za_aukcijsku_prodaju.Controllers
{
    public class AdminController : Controller
    {

        private bool checkAdmin()
        {
            bool cond = true;
            if (Session["admin"] != null)
            {
                if ((bool)Session["admin"])
                {
                    cond = false;
                }
            }

            return cond;
        }

        // GET: Admin
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string minP, string maxP, string AuctionStatus, int? page)
        {

            if (checkAdmin())
                return RedirectToAction("Login", "Account");
            else
            {
                bool onlyMin, onlyMax, minMax;
                onlyMin = onlyMax = minMax = false;

                if (!String.IsNullOrEmpty(minP)) onlyMin = true;
                if (!String.IsNullOrEmpty(maxP)) onlyMax = true;
                if (onlyMin && onlyMax)
                {
                    onlyMin = onlyMax = false;
                    minMax = true;
                }

                using (var context = new AukcijaEntities())
                {
                    ViewBag.CurrentSort = sortOrder;
                    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                    ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

                    if (searchString != null)
                    {
                        page = 1;
                    }
                    else
                    {
                        searchString = currentFilter;
                    }

                    ViewBag.CurrentFilter = searchString;

                    IEnumerable<Veb_portal_za_aukcijsku_prodaju.Models.Aukcija> aukcijas = context.Aukcijas.Include(a => a.Bid);
                    aukcijas = aukcijas.Where(s => !s.Status.Equals("DRAFT"));

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        string[] words = searchString.Split(' ');
                        aukcijas = aukcijas.Where(s => s.Proizvod.Contains(searchString));
                    }
                    try
                    {
                        if (onlyMin || onlyMax || minMax)
                        {
                            if (minMax)
                            {
                                aukcijas = aukcijas.Where(s => s.TrenutnaCena >= Double.Parse(minP) && s.TrenutnaCena <= Double.Parse(maxP));
                            }
                            else
                            {
                                if (onlyMin)
                                {
                                    aukcijas = aukcijas.Where(s => s.TrenutnaCena >= Double.Parse(minP));
                                }
                                else
                                {
                                    aukcijas = aukcijas.Where(s => s.TrenutnaCena <= Double.Parse(maxP));
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error parsing double.");
                    }

                    if (!String.IsNullOrEmpty(AuctionStatus))
                    {

                        switch (AuctionStatus)
                        {

                            case "1":
                                aukcijas = aukcijas.Where(s => s.Status == "READY");
                                break;
                            case "2":
                                aukcijas = aukcijas.Where(s => s.Status == "OPEN");
                                break;
                            case "3":
                                aukcijas = aukcijas.Where(s => s.Status == "SOLD");
                                break;
                            case "4":
                                aukcijas = aukcijas.Where(s => s.Status == "EXPIRED");
                                break;

                        }
                    }

                    switch (sortOrder)
                    {
                        case "name_desc":
                            aukcijas = aukcijas.OrderByDescending(s => s.Proizvod);
                            break;
                        case "Date":
                            aukcijas = aukcijas.OrderBy(s => s.VremeKreiranja);
                            break;
                        case "date_desc":
                            aukcijas = aukcijas.OrderByDescending(s => s.VremeKreiranja);
                            break;
                        default:  // Name ascending 
                            aukcijas = aukcijas.OrderBy(s => s.Proizvod);
                            break;
                    }


                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(aukcijas.ToPagedList(pageNumber, pageSize));
                }
            }
        }

        public ActionResult AddAuction()
        {
            if (checkAdmin())
                return RedirectToAction("Login", "Account");
            else
                return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddAuction(NewAuctionModel model)
        {
            if (ModelState.IsValid)
            {
                DateTime localDate = DateTime.Now;
                byte[] image = null;

                if (model.Slika != null)
                {
                    // Convert HttpPostedFileBase to byte array.
                    image = new byte[model.Slika.ContentLength];
                    model.Slika.InputStream.Read(image, 0, image.Length);
                }

                using (var context = new AukcijaEntities())
                {

                    var newAuction = new Aukcija()
                    {
                        Proizvod = model.Proizvod,
                        Trajanje = model.Trajanje,
                        PocetnaCena = model.PocetnaCena,
                        VremeKreiranja = localDate,
                        Status = "READY",
                        TrenutnaCena = model.PocetnaCena,
                        Slika = image
                    };

                    context.Aukcijas.Add(newAuction);
                    context.SaveChanges();
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }
    }
}