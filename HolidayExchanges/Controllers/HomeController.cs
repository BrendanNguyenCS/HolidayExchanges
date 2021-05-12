using HolidayExchanges.ViewModels;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = new MailMessage
                    {
                        From = new MailAddress(model.Email),
                        Subject = model.Subject,
                        Body = model.Message,
                        IsBodyHtml = false
                    };
                    message.To.Add("holidayexchanges.ma@gmail.com");
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
    }
}