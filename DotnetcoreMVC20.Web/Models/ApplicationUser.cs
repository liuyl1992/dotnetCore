﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DotnetcoreMVC20.Web.Models
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser
	{
		/// <summary>
		/// 昵称
		/// </summary>
		public string NickName { get; set; }
	}
}
