using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheGreatFinChallenge.Models;
using TheGreatFinChallenge.Models.Data;

namespace TheGreatFinChallenge.Xtra
{
    public class Queries
    {
        public static User GetUserByEmail(TGFCContext ctx, string email) => ctx.User
            .Include(u => u.Activities).Include(u => u.Department).Include(u => u.Images)
            .FirstOrDefault(u => u.Email == email);
        public static User GetUserById(TGFCContext ctx, int id) => ctx.User
            .Include(u => u.Activities).Include(u => u.Images).Include(u => u.Department)
            .FirstOrDefault(u => u.UserId == id);
        public static User GetUserByClaims(TGFCContext ctx, IEnumerable<Claim> claims)
        {
            Claim claim = claims.FirstOrDefault(c => c.Type == "UserId");
            return ctx.User
                .Include(u => u.Activities).Include(u => u.Images)
                .Include(u => u.Department).ThenInclude(d => d.Directorate)
                .FirstOrDefault(u => u.UserId == Convert.ToInt32(claim.Value));
        }
        public static List<User> GetAllUsersFromDirectorate(TGFCContext ctx, Directorate directorate)
        {
            return ctx.User
                .Include(u => u.Activities).Include(u => u.Images)
                .Include(u => u.Department)
                .Where(u => u.Department.Directorate == directorate).ToList();
        }

        public static Activity GetActivityById(TGFCContext ctx, int id) => ctx.Activity
            .Include(a => a.User).Include(a => a.ActivityType)
            .FirstOrDefault(a => a.ActivityId == id);
        public static List<Activity> GetActivitiesOfUser(TGFCContext ctx, User u) => ctx.Activity
            .Include(a => a.User).Include(a => a.ActivityType)
            .Where(a => a.User == u).ToList();
        public static List<ActivityType> GetAllActivityTypes(TGFCContext ctx) => ctx.ActivityType
            .Include(a => a.Discipline).Include(a => a.Activities)
            .ToList();

        public static Discipline GetDisciplineById(TGFCContext ctx, int id) => ctx.Discipline
            .Include(d => d.ActivityTypes)
            .FirstOrDefault(d => d.DisciplineId == id);
        public static List<Discipline> GetAllDisciplines(TGFCContext ctx) => ctx.Discipline
            .Include(d => d.ActivityTypes)
            .ToList();

        public static Directorate GetDirectorateById(TGFCContext ctx, int id) => ctx.Directorate
            .Include(d => d.Departments)
            .FirstOrDefault(d => d.DirectorateId == id);
        public static List<Directorate> GetAllDirectorates(TGFCContext ctx) => ctx.Directorate
            .Include(d => d.Departments)
            .ToList();

        public static Department GetDepartmentById(TGFCContext ctx, int id) => ctx.Department
            .Include(d => d.Directorate).Include(d => d.Users)
            .FirstOrDefault(d => d.DepartmentId == id);
        public static List<Department> GetAllDepartmentsOfDirectorate(TGFCContext ctx, Directorate directorate) => ctx.Department
            .Include(d => d.Directorate).Include(d => d.Users)
            .Where(d => d.Directorate == directorate).ToList();

        public static List<Image> GetAllImagesOfDirectorate(TGFCContext ctx, Directorate directorate) => ctx.Image
            .Include(i => i.User)
            .Where(i => i.User.Department.Directorate == directorate).ToList();
        public static Image getImageById(TGFCContext ctx, int imageId) => ctx.Image
            .Include(i => i.User)
            .FirstOrDefault(i => i.ImageId == imageId);
    }
}
