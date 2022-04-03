using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheGreatFinChallenge.Models.Data;
using TheGreatFinChallenge.Xtra;

namespace TheGreatFinChallenge.Models.Views
{
    public class ImagesView
    {
        public User CurrentUser { get; set; }
        public List<Image> Images = new List<Image>();
        public List<Department> Departments = new List<Department>();

        public ImagesView(TGFCContext _ctx, IEnumerable<Claim> _claims)
        {
            CurrentUser = Queries.GetUserByClaims(_ctx, _claims);
            Images = Queries.GetAllImagesOfDirectorate(_ctx, CurrentUser.Department.Directorate);
            Departments = Queries.GetAllDepartmentsOfDirectorate(_ctx, CurrentUser.Department.Directorate);
        }
    }
}
