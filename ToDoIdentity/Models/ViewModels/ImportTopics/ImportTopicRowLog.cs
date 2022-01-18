using ToDoIdentity.Models.Attributes;
using ToDoIdentity.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;

namespace ToDoIdentity.Models
{
    public class ImportTopicRowLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public ImportTopicRowLogType Type { get; set; }
    }
}