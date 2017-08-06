using System;
using Microsoft.EntityFrameworkCore;
using DotnetCore.Entity;
using System.Threading.Tasks;

namespace DotNetCore.Service
{
	public class DemoService : IDemoService
	{
		private readonly IUnitOfWork _unitOfWork;

		public DemoService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Course> Method()
		{
			var repo = _unitOfWork.GetRepository<Course>();
			var values = await repo.Query(x => true, true).ToListAsync();
			return values.FindLast(s=>s.CourseID!=0);
		}
	}
}
