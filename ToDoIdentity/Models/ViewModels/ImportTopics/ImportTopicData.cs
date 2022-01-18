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
    public class ImportTopicData
    {
        public string Name { get; set; }        
    }
}