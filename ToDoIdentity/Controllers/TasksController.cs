using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using ClosedXML.Excel;
using Rotativa;
using ToDoIdentity.Extensions;
using ToDoIdentity.Models;
using ToDoIdentity.Models.Xml;

namespace ToDoIdentity.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var db = new ToDoContext();
            var tasks = db.Tasks.ToList();

            return View(tasks);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            var task = new Task();
            return View(task);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(Task model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var db = new ToDoContext();

           

            if (model.TopicIds != null && model.TopicIds.Any())
            {
                var topic = db.Topics.Where(s => model.TopicIds.Contains(s.Id)).ToList();
                model.Topics = topic;
            }
            if (model.DayOfWeekIds != null && model.DayOfWeekIds.Any())
            {
                var dayOfWeeks = db.DayOfWeeks.Where(s => model.DayOfWeekIds.Contains(s.Id)).ToList();
                model.DayOfWeeks = dayOfWeeks;
            }
            if (!ModelState.IsValid)
                return View(model);



            db.Tasks.Add(model);
            db.SaveChanges();

            return RedirectPermanent("/Tasks/Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            var db = new ToDoContext();
            var task = db.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return RedirectPermanent("/Tasks/Index");

            db.Tasks.Remove(task);
            db.SaveChanges();

            return RedirectPermanent("/Tasks/Index");
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            var db = new ToDoContext();
            var task = db.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return RedirectPermanent("/Tasks/Index");

            return View(task);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(Task model)
        {
            var db = new ToDoContext();
            var task = db.Tasks.FirstOrDefault(x => x.Id == model.Id);
            if (task == null)
                ModelState.AddModelError("Id", "Задача не найдена");
           
            if (!ModelState.IsValid)
                return View(model);

            MappingTask(model, task, db);

            db.Entry(task).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectPermanent("/Tasks/Index");
        }

        private void MappingTask(Task sourse, Task destination, ToDoContext db)
        {
            destination.Name = sourse.Name;
            destination.PerformerId = sourse.PerformerId;
            destination.Performer = sourse.Performer;
            

            if (destination.Topics != null)
                destination.Topics.Clear();

            if (sourse.TopicIds != null && sourse.TopicIds.Any())
                destination.Topics = db.Topics.Where(s => sourse.TopicIds.Contains(s.Id)).ToList();

            if (destination.DayOfWeeks != null)
                destination.DayOfWeeks.Clear();

            if (sourse.DayOfWeekIds != null && sourse.DayOfWeekIds.Any())
                destination.DayOfWeeks = db.DayOfWeeks.Where(s => sourse.DayOfWeekIds.Contains(s.Id)).ToList();
        }

        [HttpGet]
        public ActionResult GetImage(int id)
        {
            var db = new ToDoContext();
            var image = db.PerformerImages.FirstOrDefault(x => x.Id == id);
            if (image == null)
            {
                FileStream fs = System.IO.File.OpenRead(Server.MapPath(@"~/Content/Images/not-foto.png"));
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                fs.Close();

                return File(new MemoryStream(fileData), "image/jpeg");
            }

            return File(new MemoryStream(image.Data), image.ContentType);
        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            var db = new ToDoContext();
            var task = db.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return RedirectPermanent("/Tasks/Index");

            return View(task);
        }

        [HttpGet]
        public ActionResult Pdf(int id)
        {
            var db = new ToDoContext();
            var task = db.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return RedirectPermanent("/Tasks/Index");

            var pdf = new ViewAsPdf("Pdf", task);
            var data = pdf.BuildFile(this.ControllerContext);


            return File(new MemoryStream(data), "application/pdf", "document.pdf");
        }

        [HttpGet]
        public FileResult GetXlsx(Task model)
        {
            var db = new ToDoContext();
            var values = db.Tasks.ToList();
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Data");


            ws.Cell("A" + 1).Value = "Id";
            ws.Cell("B" + 1).Value = "Задача пары";
            ws.Cell("C" + 1).Value = "Тема(ы)";
            ws.Cell("D" + 1).Value = "Исполнитель";
            ws.Cell("E" + 1).Value = "Важность";

            ws.Cell("F" + 1).Value = "Описание";
            ws.Cell("G" + 1).Value = "Примерное время выполнения";
            ws.Cell("H" + 1).Value = "День недели";
            

            int row = 2;
            foreach (var value in values)
            {
                ws.Cell("A" + row).Value = value.Id;
                ws.Cell("B" + row).Value = value.Name;
                ws.Cell("C" + row).Value = string.Join(", ", value.Topics.Select(x => $"{x.Name}"));
                ws.Cell("D" + row).Value = value.Performer.Name;
                ws.Cell("E" + row).Value = value.Importance.GetDisplayValue();

                ws.Cell("F" + row).Value = value.Description;
                ws.Cell("G" + row).Value = value.Time;
                ws.Cell("H" + row).Value = string.Join(", ", value.DayOfWeeks.Select(x => $"{x.DayOfWeekName}"));
                
                row++;
            };
            var rngHead = ws.Range("A1:H" + 1);
            rngHead.Style.Fill.BackgroundColor = XLColor.AshGrey;

            var rngTable = ws.Range("A1:H" + 10);
            rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Columns().AdjustToContents();



            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Tasks.xlsx");
            }
        }

        [HttpGet]
        public ActionResult GetXml()
        {
            var db = new ToDoContext();
            var tasks = db.Tasks.ToList().Select(x => new XmlTask()
            {
                Name = x.Name,
                PerformerId = x.PerformerId,
                Importance = x.Importance,
                Description = x.Description,
                Time = x.Time,
                Topics = x.Topics.Select(y => new XmlTopic() { Id = y.Id }).ToList(),
                DayOfWeeks = x.DayOfWeeks.Select(y => new XmlDayOfWeek() { Id = y.Id }).ToList()
            }).ToList();

            XmlSerializer xml = new XmlSerializer(typeof(List<XmlTask>));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var ms = new MemoryStream();
            xml.Serialize(ms, tasks, ns);
            ms.Position = 0;

            return File(new MemoryStream(ms.ToArray()), "text/xml");
        }


    }
}