using HolidayExchanges.ViewModels;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() => View();

        public ActionResult About() => View();

        [HttpGet]
        public ActionResult Contact() => View();

        [HttpPost]
        public async Task<ActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = new MailMessage
                    {
                        From = new MailAddress("holidayexchanges.ma@gmail.com", "Holiday Exchanges MA"),
                        Subject = model.Subject,
                        Body = model.Message,
                        IsBodyHtml = false
                    };
                    message.To.Add(model.Email);
                    using (var smtp = new SmtpClient())
                    {
                        await smtp.SendMailAsync(message);
                    }

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = $"Error. Please try again later.\n{e.Message}";
                    return View("Error");
                }
            }

            return View(model);
        }

        // just a discovery method for sending emails with Razor template
        public async Task<ActionResult> SendEmail(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                string body = string.Empty;
                using (StreamReader reader = new StreamReader("~/Views/Templates/ContactEmail.cshtml"))
                {
                    body = reader.ReadToEnd();
                    //Replace UserName and Other variables available in body Stream
                    body = body.Replace("{ContactName}", model.Name);
                    body = body.Replace("{Message}", model.Message);
                }

                var message = new MailMessage
                {
                    From = new MailAddress("holidayexchanges.ma@gmail.com", "Holiday Exchanges MA"),
                    Subject = model.Subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(model.Email);

                try
                {
                    using (var smtp = new SmtpClient())
                    {
                        await smtp.SendMailAsync(message);
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = $"Error. Please try again later.\n{e.Message}";
                    return View("Error");
                }

                return new EmptyResult();
            }

            return View("Error");
        }
    }
}