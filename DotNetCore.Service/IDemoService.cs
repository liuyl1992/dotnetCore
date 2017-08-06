using DotnetCore.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Service
{
	public interface IDemoService
	{
		Task<Course> Method();
	}
}
