using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using ToDoIdentity.Models;

namespace ToDoIdentity.Controllers
{
    public class ImportTopicsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new ImportTopicViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Import(ImportTopicViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var log = ProceedImport(model);

            return View("Log", log);
        }

        public ActionResult GetExample()
        {
            return File("~/Content/Files/ImportTopicsExample.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ImportTopicsExample.xlsx");
        }

        private ImportTopicLog ProceedImport(ImportTopicViewModel model)
        {
            var startTime = DateTime.Now;

            var workBook = new XLWorkbook(model.FileToImport.InputStream);
            var workSheet = workBook.Worksheet(1);
            var rows = workSheet.RowsUsed().Skip(1).ToList();

            var logs = new List<ImportTopicRowLog>();
            var data = ParseRows(rows, logs);
            ApplyImported(data);

            var successCount = data.Count();
            var failedCount = rows.Count() - successCount;
            var finishTime = DateTime.Now;

            var result = new ImportTopicLog()
            {
                StartImport = startTime,
                EndImport = finishTime,
                SuccessCount = successCount,
                FailedCount = failedCount,
                Logs = logs
            };

            return result;
        }

        private List<ImportTopicData> ParseRows(IEnumerable<IXLRow> rows, List<ImportTopicRowLog> logs)
        {
            var result = new List<ImportTopicData>();
            int index = 1;
            foreach (var row in rows)
            {
                try
                {
                    var data = new ImportTopicData()
                    {
                        Name = ConvertToString(row.Cell("A").GetValue<string>().Trim()),                        
                    };

                    result.Add(data);
                    logs.Add(new ImportTopicRowLog()
                    {
                        Id = index,
                        Message = $"ОК",
                        Type = ImportTopicRowLogType.Success
                    }); ;

                }
                catch (Exception ex)
                {
                    logs.Add(new ImportTopicRowLog()
                    {
                        Id = index,
                        Message = $"Error: {ex.GetBaseException().Message}",
                        Type = ImportTopicRowLogType.ErrorParsed
                    }); ;
                }

                index++;
            }


            return result;
        }

        private void ApplyImported(List<ImportTopicData> data)
        {
            var db = new ToDoContext();

            foreach (var value in data)
            {
                var model = new Topic()
                {
                    Name = value.Name,            
                };

                db.Topics.Add(model);
                db.SaveChanges();
            }
        }

        private string ConvertToString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new Exception("Значение не определено");

            var result = HandleInjection(value);

            return result;
        }
        private string HandleInjection(string value)
        {
            var badSymbols = new Regex(@"^[+=@-].*");
            return Regex.IsMatch(value, badSymbols.ToString()) ? string.Empty : value;
        }

        private DateTime? ConvertToDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            DateTime result = default;

            if (DateTime.TryParse(value, out DateTime temp))
                result = temp;

            if (result == default)
                return null;

            return result;
        }
    }
}