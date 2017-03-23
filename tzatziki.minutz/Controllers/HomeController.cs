﻿using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using tzatziki.minutz.interfaces;
using tzatziki.minutz.models;

namespace tzatziki.minutz.Controllers
{
	[Authorize]
	public class HomeController : BaseController
  {

		//private readonly IInstanceRepository _instanceRepository;

		public HomeController(
			ITokenStringHelper tokenStringHelper, 
			IProfileService profileService, 
			//IInstanceRepository instanceRepository, 
			IOptions<AppSettings> settings): base(settings, profileService, tokenStringHelper)
		{

			//_instanceRepository = instanceRepository;
		}


	
		public IActionResult Index()
		{
      this.UserProfile = User.ToProfile(ProfileService, TokenStringHelper, AppSettings);

      var model = new CalenderModel { };
			
			return View(model);
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}
