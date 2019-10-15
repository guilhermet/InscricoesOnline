using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.Security;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;
using InscricoesOnline.Models;
using InscricoesOnline.Security;
using InscricoesOnline.ViewModel;

namespace InscricoesOnline.Controllers
{
    public class AccountController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/Account/Login")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Account/Login")]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            var usuario = db.Eventos.Where(u => u.Login == model.Email && u.Senha == model.Password && u.Ativo).FirstOrDefault();

            if (usuario != null)
            {
                AdminSessionPersister.Username = usuario.Login;
                if (model.RememberMe)
                {
                    Response.Cookies.Get("username").Value = AdminSessionPersister.Username;
                    Response.Cookies.Get("username").Expires = DateTime.Now.AddYears(2);
                }

                if (!String.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.Erro = "Usuário ou senha inválida";
                return View(model);
            }
        }


        [Route("Admin/Account/EsqueciMinhaSenha")]
        public ActionResult EsqueciMinhaSenha()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Account/EsqueciMinhaSenha")]
        public ActionResult EsqueciMinhaSenha(ForgotPasswordViewModel model)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("fptkd.no.reply@gmail.com");
            mail.To.Add("guilhermetk92@gmail.com"); // para
            mail.Subject = "Teste"; // assunto
            mail.Body = "Testando mensagem de e-mail"; // mensagem

            using (var smtp = new SmtpClient("smtp.gmail.com"))
            {
                smtp.EnableSsl = true; // GMail requer SSL
                smtp.Port = 587;       // porta para SSL
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // modo de envio
                smtp.UseDefaultCredentials = false; // vamos utilizar credencias especificas
                smtp.Credentials = new NetworkCredential("fptkd.no.reply@gmail.com", "fptkd2018*");

                smtp.Send(mail);
            }

            return View(model);
        }

        [AllowAnonymous]
        [Route("Admin/Account/ResetarSenha")]
        public ActionResult ResetarSenha()
        {
            if (String.IsNullOrEmpty(AdminSessionPersister.Username))
            {
                View("Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Account/ResetarSenha")]
        public ActionResult ResetarSenha(ResetPasswordViewModel model)
        {
            var usuario = db.Eventos.Find(AdminSessionPersister.Evento.Id);
            usuario.Senha = model.Password;

            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            AdminSessionPersister.Username = string.Empty;
            if (Response.Cookies["username"] != null)
            {
                HttpCookie authenticationCookie = new HttpCookie("username");
                authenticationCookie.Expires = DateTime.Now.AddDays(-1);
                authenticationCookie.Value = AdminSessionPersister.Username;
                Response.Cookies.Add(authenticationCookie);
            }
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}