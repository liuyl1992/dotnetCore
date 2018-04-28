using System;
using Microsoft.EntityFrameworkCore;
using DotnetCore.Entity;
using System.Threading.Tasks;
using System.Linq;

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
            var repoStudent = _unitOfWork.GetRepository<Student>();
            repoStudent.GetFirstOrDefault();
            var values =await Task.Run(() => {return repo.GetAll(); });
            return values.FirstOrDefault();
		}
	}
}
