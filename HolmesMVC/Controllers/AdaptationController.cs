﻿namespace HolmesMVC.Controllers
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI;
    using HolmesMVC.Enums;
    using HolmesMVC.Models;
    using HolmesMVC.Models.ViewModels;

    [OutputCache(Duration = 86400, Location = OutputCacheLocation.Server, VaryByCustom = "LastDbUpdate")]
    public class AdaptationController : HolmesDbController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            AdaptListView model = new AdaptListView(Db);

            return View(model);
        }

        //
        // GET: /radio/merrison

        [AllowAnonymous]
        public ActionResult RadioDetails(string urlName = "")
        {
            Adaptation adaptation = Db.Adaptations.Where(a => a.UrlName == urlName).FirstOrDefault();
            if (adaptation == null)
            {
                return RedirectToAction("Details", "Adaptation", new { urlName });
            }

            var viewmodel = new AdaptView(adaptation);

            if (viewmodel.Medium != (int)Medium.Radio)
            {
                return RedirectToAction("Details", "Adaptation", new { viewmodel.UrlName });
            }
            return View("Details", viewmodel);
        }

        //
        // GET: /film/without_a_clue

        [AllowAnonymous]
        public ActionResult SingleFilmDetails(string urlName = "")
        {
            Adaptation adaptation = Db.Adaptations.Where(a => a.UrlName == urlName).FirstOrDefault();
            if (adaptation == null)
            {
                return RedirectToAction("Details", "Adaptation", new { urlName });
            }

            var viewmodel = new AdaptView(adaptation);

            if (!viewmodel.SingleFilm)
            {
                return RedirectToAction("Details", "Adaptation", new { viewmodel.UrlName });
            }
            if (viewmodel.Medium == (int)Medium.Radio)
            {
                return RedirectToAction("RadioDetails", "Adaptation", new { viewmodel.UrlName });
            }
            return View(viewmodel);
        }

        //
        // GET: /tv/granada

        [AllowAnonymous]
        public ActionResult TVDetails(string urlName = "")
        {
            Adaptation adaptation = Db.Adaptations.Where(a => a.UrlName == urlName).FirstOrDefault();
            if (adaptation == null)
            {
                return RedirectToAction("Details", "Adaptation", new { urlName });
            }

            var viewmodel = new AdaptView(adaptation);

            if (viewmodel.SingleFilm)
            {
                return RedirectToAction("SingleFilmDetails", "Adaptation", new { viewmodel.UrlName });
            }
            if (viewmodel.Medium != (int)Medium.Television)
            {
                return RedirectToAction("Details", "Adaptation", new { viewmodel.UrlName });
            }
            return View("Details", viewmodel);
        }

        //
        // GET: /adaptation/wontner

        [AllowAnonymous]
        public ActionResult Details(string urlName = "")
        {
            if (string.IsNullOrEmpty(urlName))
            {
                return HttpNotFound();
            }
            if (urlName == "canon")
            {
                return RedirectToActionPermanent("Index", "Canon");
            }

            int id = 0;
            if (int.TryParse(urlName, out id))
            {
                var adaptationById = Db.Adaptations.Find(id);
                if (adaptationById != null)
                {
                    return RedirectToActionPermanent("Details", "Adaptation", new { adaptationById.UrlName });
                }
            }

            Adaptation adaptation = Db.Adaptations.Where(a => a.UrlName == urlName).FirstOrDefault();
            if (adaptation == null)
            {
                return HttpNotFound();
            }

            var actionName = "Details";
            switch (adaptation.MediumUrlName)
            {
                case "radio":
                    actionName = "RadioDetails";
                    break;
                case "film":
                    actionName = "SingleFilmDetails";
                    break;
                case "tv":
                    actionName = "TVDetails";
                    break;
            }
            if (actionName != "Details")
            {
                return RedirectToActionPermanent(actionName, "Adaptation", new { urlName });
            }
            
            var viewmodel = new AdaptView(adaptation);
            return View(viewmodel);
        }

        //
        // GET: /adaptation/create

        public ActionResult Create()
        {
            ViewBag.Medium = new SelectList(
                Enum.GetNames(typeof(Medium))
                    .Select(m => new
                    {
                        ID = (int)Enum.Parse(typeof(Medium), m),
                        Name = m
                    })
                , "ID", "Name", string.Empty);
            return View();
        }

        //
        // POST: /adaptation/create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Adaptation adaptation)
        {
            if (ModelState.IsValid)
            {
                Db.Adaptations.Add(adaptation);
                Db.SaveChanges(); Shared.SomethingChanged(HttpContext.Application);

                return RedirectToAction("Details", "Adaptation", new { adaptation.UrlName });
            }

            ViewBag.Medium = new SelectList(
                Enum.GetNames(typeof(Medium))
                    .Select(m => new
                    {
                        ID = (int)Enum.Parse(typeof(Medium), m),
                        Name = m
                    })
                , "ID", "Name", adaptation.Medium);
            return View(adaptation);
        }

        //
        // GET: /adaptation/edit/5

        public ActionResult Edit(int id = 0)
        {
            Adaptation adaptation = Db.Adaptations.Find(id);
            if (adaptation == null)
            {
                return HttpNotFound();
            }
            ViewBag.Medium = new SelectList(
                Enum.GetNames(typeof(Medium))
                    .Select(m => new
                    {
                        ID = (int)Enum.Parse(typeof(Medium), m),
                        Name = m
                    })
                , "ID", "Name", adaptation.Medium);
            return View(adaptation);
        }

        //
        // POST: /adaptation/edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Adaptation adaptation)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(adaptation).State = EntityState.Modified;
                Db.SaveChanges(); Shared.SomethingChanged(HttpContext.Application);

                return RedirectToAction("Details", new { adaptation.ID });
            }
            ViewBag.Medium = new SelectList(                
                Enum.GetNames(typeof(Medium))
                    .Select(m => new
                    {
                        ID = (int)Enum.Parse(typeof(Medium), m),
                        Name = m
                    })                
                , "ID", "Name", adaptation.Medium);
            return View(adaptation);
        }

        //
        // GET: /adaptation/delete/5

        public ActionResult Delete(int id = 0)
        {
            Adaptation adaptation = Db.Adaptations.Find(id);
            if (adaptation == null)
            {
                return HttpNotFound();
            }
            if (adaptation.Seasons.Any())
            {
                return View("CantDelete", adaptation);
            }
            return View(adaptation);
        }

        //
        // POST: /adaptation/delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Adaptation adaptation = Db.Adaptations.Find(id);
            Db.Adaptations.Remove(adaptation);
            Db.SaveChanges(); Shared.SomethingChanged(HttpContext.Application);

            return RedirectToAction("Index","Home");
        }
    }
}