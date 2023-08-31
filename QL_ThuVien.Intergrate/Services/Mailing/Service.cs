using QL_ThuVien.Intergrate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace QL_ThuVien.Intergrate.Services.Mailing
{
    public class Service
    {
        private string username, password;
        #region SingleTon Patterm        
        private static Service instance;       
        private Service(){}
        public static Service Instance { get => instance?? new Service(); private set => instance = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        #endregion
        #region Sender
        /// <summary>
        /// Mail Sender
        /// </summary>
        /// <param name="mailObj"></param>
        public void MailSender(SMail mailObj)
        {
            MailMessage mail = new MailMessage();
            SetupMail(ref mail, mailObj);
            SmtpClient smtp = new SmtpClient();
            SetupSmtpClient(ref smtp);
            smtp.Send(mail);
        }
        /// <summary>
        /// Use for mail configuration
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="mailObj"></param>
        public void SetupMail(ref MailMessage mail, SMail mailObj)
        {
            mail.To.Add(mailObj.To);
            mail.From = new MailAddress("noreply.QLTV@gmail.com");
            mail.Subject = mailObj.Subject;
            mail.Body = mailObj.Body;
            mail.IsBodyHtml = true;
        }
        /// <summary>
        /// Use for setup Smtp Client (Do not change this method!)
        /// </summary>
        /// d2USih1asAJD
        /// 3GsFUy4!e0!J
        /// pO9uaDKs@45$
        /// <param name="smtp"></param>
        public void SetupSmtpClient(ref SmtpClient smtp)
        {
            Username = "noreply.QLTV@gmail.com";
            Password = "fpwqwihtowquhepv";//App password
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential(username, password);
            smtp.EnableSsl = true;
        }
        #endregion
    }
}
