using System.Collections.Generic;
using System.Xml.Serialization;
using ToDoIdentity.Models.Xml;

namespace ToDoIdentity.Models
{
    [XmlRoot("Task")]
    public class XmlTask
    {
        /// <summary>
        /// Задача
        /// </summary>    
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Тема
        /// </summary> 
        [XmlArray("Topics")]
        [XmlArrayItem(typeof(XmlTopic), ElementName = "Topic")]
        public virtual List<XmlTopic> Topics { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary> 
        [XmlElement("PerformerId")]
        public int PerformerId { get; set; }

        /// <summary>
        /// Важность
        /// </summary> 
        [XmlElement("Importance")]
        public Importance Importance { get; set; }

        //<summary>
        //Описание
        //</summary>    
        [XmlElement("Description")]
        public string Description { get; set; }

        //<summary>
        //Примерное время выполнения
        //</summary>    
        [XmlElement("Time")]
        public string Time { get; set; }

        /// <summary>
        /// День недели
        /// </summary> 
        [XmlArray("DayOfWeeks")]
        [XmlArrayItem(typeof(XmlDayOfWeek), ElementName = "DayOfWeek")]
        public virtual List<XmlDayOfWeek> DayOfWeeks { get; set; }

    }
}