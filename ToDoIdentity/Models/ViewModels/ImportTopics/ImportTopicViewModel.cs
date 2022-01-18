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
    public class ImportTopicViewModel
    {
        /// <summary>
        /// Id
        /// </summary> 
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }


        [Display(Name = "Файл импорта")]
        [Required(ErrorMessage = "Укажите файл импорта (.xlsx)")]
        public HttpPostedFileBase FileToImport { get; set; }
    }
}