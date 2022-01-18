using System.Xml.Serialization;

namespace ToDoIdentity.Models.Xml
{
    public class XmlDayOfWeek
    {
        /// <summary>
        /// Id
        /// </summary> 
        [XmlElement("Id")]
        public int Id { get; set; }
    }
}