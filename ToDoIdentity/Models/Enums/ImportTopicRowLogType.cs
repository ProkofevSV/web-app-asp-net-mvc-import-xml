using System.ComponentModel.DataAnnotations;

namespace ToDoIdentity.Models
{
    public enum ImportTopicRowLogType
    {
        [Display(Name = "Успешно")]
        Success = 1,

        [Display(Name = "Ошибка при парсинге строки")]
        ErrorParsed = 2,
    }
}