using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using ToDoIdentity.Extensions;
using ToDoIdentity.Models;

namespace ToDoIdentity.Controllers
{
    public class ImportXmlTasksController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new ImportXmlTaskViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Import(ImportXmlTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var file = new byte[model.FileToImport.InputStream.Length];
            model.FileToImport.InputStream.Read(file, 0, (int)model.FileToImport.InputStream.Length);

            XmlSerializer xml = new XmlSerializer(typeof(List<XmlTask>));
            var tasks = (List<XmlTask>)xml.Deserialize(new MemoryStream(file));
            var db = new ToDoContext();



            foreach (var task in tasks)
            {
                var input = new Task()
                {
                    Name = task.Name,
                    PerformerId = task.PerformerId,
                    Importance = task.Importance,
                    Description = task.Description,
                    Time = task.Time,
                };
                var topicIds = task.Topics.Select(x => x.Id).ToList();
                var topics = db.Topics.Where(s => topicIds.Contains(s.Id)).ToList();
                input.Topics = topics;

                var dayofweekIds = task.DayOfWeeks.Select(x => x.Id).ToList();
                var dayofweeks = db.DayOfWeeks.Where(s => dayofweekIds.Contains(s.Id)).ToList();
                input.DayOfWeeks = dayofweeks;

                db.Tasks.Add(input);

                db.SaveChanges();
            }

            return RedirectPermanent("/Tasks/Index");
        }

        public ActionResult GetExample()
        {
            return File("~/Content/Files/ImportXmlTasksExample.xml", "application/xml", "ImportXmlTasksExample.xml");
        }

    }
}