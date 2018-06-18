﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;



namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today,
            };

            //ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
            SetupActivitiesSelectListItems();
            return View(entry);
        }
        
        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            //ModelState.AddModelError("", "This is a global message");


            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                return RedirectToAction("Index");
            }

            // TODO Display the Entries list page
            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ToDo - get the requested entry from the repository, also make sure an entry is found, and if not, 

            Entry entry = _entriesRepository.GetEntry((int)id);

            //ToDo -  return a status of not found if the entry was not found

            if (entry == null)
            {
                return HttpNotFound();
            }

            // todo - populate the activities select list items ViewBag property
            SetupActivitiesSelectListItems();
            //ToDo - pass the entry into the view

            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            //ToDo - validate the entry 
            ValidateEntry(entry);  


            //To do - if the entry is valid 
            // 1)  Use the repository to update the entry
            // 2) redirect the user to the "entries list page"
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);

                return RedirectToAction("Index");
            }

            SetupActivitiesSelectListItems();

            // To do:  populate the activities select listitems ViewBag Property

            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //ToDo- Retrieve entry for the provided if parameter value
            Entry entry = _entriesRepository.GetEntry((int)id);

            //ToDo - Return "not found if the entry was not found
            if (entry == null)
            {
                return HttpNotFound();
            }

            //Todo - pass the entry to the view
            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete (int id)
        {

            // To do - Delete the entry
            _entriesRepository.DeleteEntry(id);

            // todo - Redirect to the "Entries" list page
             
            return RedirectToAction("Index"); 
        }

        private void ValidateEntry(Entry entry)
        {
            // If there are not any "Duration" field validation errors
            // then make sure that the duration is greater than "0"
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration",
                    "The Duration field value must be greater than '0'.");
            }
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}