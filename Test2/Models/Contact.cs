using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Test2.Models
{
    public class Contact
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string NoiDung { get; set; }

        public DateTime SentDate { get; set; }
        public string IP { get; set; }

        public string BuildMessage()
        {
            var message = new StringBuilder();
            message.AppendFormat("Date: {0:yyyy-MM-dd hh:mm}\n", SentDate);
            message.AppendFormat("Email from: {0}\n", Email);
            message.AppendFormat("Email: {0}\n", Email);
            message.AppendFormat("Name: {0}\n", Name);
            message.AppendFormat("Phone: {0}\n", Phone);
            message.AppendFormat("IP: {0}\n", IP);
            message.AppendFormat("NoiDung: {0}\n", NoiDung);
            return message.ToString();
        }
    }
}