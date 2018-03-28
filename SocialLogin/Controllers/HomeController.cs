using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SocialLogin.Controllers
{
	public class HomeController : Controller
	{
		#region ------------- fb declaration -----------------
		private static string ClientId = "1960234210876856"; // App Id
		private static string ClientSecretKey = "5371d17070830310ca9b21a2f8ae8e1d"; // App Secret Key
		#endregion

		#region -------------------- Google --------------------
		public ActionResult Index()
		{
			ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

			return View();
		}

		//public ActionResult About()
		//{
		//	ViewBag.Message = "Your app description page.";

		//	return View();
		//}

		[HttpPost]
		public JsonResult About(string Email, string FirstName, string LastName, string GoogleID, string ProfileURL)
		{
			//Do your code for Signin or Signup  
			return Json(true, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region ---------------------- Facebook ----------------------
		[HttpGet]
		public ActionResult FacebookLogin()
		{
			var fb = new FacebookClient();
			var loginUrl = fb.GetLoginUrl(new
			{
				client_id = ClientId,
				client_secret = ClientSecretKey,
				redirect_uri = RedirectUri.AbsoluteUri,
				response_type = "code",
				scope = "email" // Add other permissions as needed
			});

			//return Redirect(loginUrl.AbsoluteUri);
			return Json(new { result = "Redirect", url = loginUrl.AbsoluteUri }, JsonRequestBehavior.AllowGet);
		}
		public ActionResult FacebookCallBack(string code)
		{
			try
			{
				var fb = new FacebookClient();
				dynamic result = fb.Post("oauth/access_token", new
				{
					client_id = ClientId,
					client_secret = ClientSecretKey,
					redirect_uri = RedirectUri.AbsoluteUri,
					code = code
				});

				var accessToken = result.access_token;

				// Store the access token in the session for farther use
				Session["AccessToken"] = accessToken;

				// update the facebook client with the access token so 
				// we can make requests on behalf of the user
				fb.AccessToken = accessToken;

				// Get the user's information
				dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
				string email = me.email;
				string firstname = me.first_name;
				string middlename = me.middle_name;
				string lastname = me.last_name;

				// Set the auth cookie
				FormsAuthentication.SetAuthCookie(email, false);
				return RedirectToAction("Index", "Home");
			}
			catch(Exception ex)
			{
				return RedirectToAction("Index", "Home");
			}
		}

		private Uri RedirectUri
		{
			get
			{
				var uriBuilder = new UriBuilder(Request.Url);
				uriBuilder.Query = null;
				uriBuilder.Fragment = null;
				uriBuilder.Path = Url.Action("FacebookCallback");
				return uriBuilder.Uri;
			}
		}
		#endregion
	}
}