﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ToDoIdentity.Models;
using DayOfWeek = ToDoIdentity.Models.DayOfWeek;

namespace ToDoIdentity.Controllers
{
    [Authorize]
    public class DayOfWeeksController: Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var db = new ToDoContext();
            var dayOfWeeks = db.DayOfWeeks.ToList();
            return View(dayOfWeeks);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            var dayOfWeek = new DayOfWeek();

            return View(dayOfWeek);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(DayOfWeek model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var db = new ToDoContext();

            db.DayOfWeeks.Add(model);
            db.SaveChanges();


            return RedirectPermanent("/DayOfWeeks/Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            var db = new ToDoContext();
            var dayOfWeek = db.DayOfWeeks.FirstOrDefault(x => x.Id == id);
            if (dayOfWeek == null)
                return RedirectPermanent("/DayOfWeeks/Index");

            db.DayOfWeeks.Remove(dayOfWeek);
            db.SaveChanges();

            return RedirectPermanent("/DayOfWeeks/Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            var db = new ToDoContext();
            var dayOfWeek = db.DayOfWeeks.FirstOrDefault(x => x.Id == id);
            if (dayOfWeek == null)
                return RedirectPermanent("/DayOfWeeks/Index");

            return View(dayOfWeek);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(DayOfWeek model)
        {

            var db = new ToDoContext();
            var dayOfWeek = db.DayOfWeeks.FirstOrDefault(x => x.Id == model.Id);
            if (dayOfWeek == null)
            {
                ModelState.AddModelError("Id", "Группа не найдена");
            }
            if (!ModelState.IsValid)
                return View(model);

            MappingDayOfWeek(model, dayOfWeek);


            db.Entry(dayOfWeek).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectPermanent("/DayOfWeeks/Index");
        }

        private void MappingDayOfWeek(DayOfWeek sourse, DayOfWeek destination)
        {
            destination.DayOfWeekName = sourse.DayOfWeekName;
        }
    }
}