using Ankets.Data;
using Microsoft.EntityFrameworkCore;

namespace Ankets.Services
{
	public interface IPermissionService
	{
		Task<bool> IsAdminAsync(int userId);
		Task<bool> IsSurveyOwnerAsync(int surveyId, int userId);
		Task<bool> CanManageSurveyAsync(int surveyId, int userId);
		Task<string> GetMyRoleForSurveyAsync(int surveyId, int userId);
	}

	public class PermissionService : IPermissionService
	{
		private readonly AppDbContext _context;

		public PermissionService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<bool> IsAdminAsync(int userId)
		{
			var roleName = await _context.Users
				.Where(u => u.UsersId == userId)
				.Select(u => u.Role!.RoleName)
				.FirstOrDefaultAsync();

			return string.Equals(roleName, "admin", StringComparison.OrdinalIgnoreCase);
		}

		public Task<bool> IsSurveyOwnerAsync(int surveyId, int userId)
		{
			return _context.Surveys.AnyAsync(s => s.SurveyId == surveyId && s.CreatorId == userId);
		}

		public async Task<bool> CanManageSurveyAsync(int surveyId, int userId)
		{
			return await IsAdminAsync(userId) || await IsSurveyOwnerAsync(surveyId, userId);
		}

		public async Task<string> GetMyRoleForSurveyAsync(int surveyId, int userId)
		{
			if (await IsAdminAsync(userId)) return "admin";
			if (await IsSurveyOwnerAsync(surveyId, userId)) return "owner";
			return "user";
		}
	}
}


