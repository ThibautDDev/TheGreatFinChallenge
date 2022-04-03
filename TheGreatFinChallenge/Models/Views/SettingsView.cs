using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheGreatFinChallenge.Models.Data;
using TheGreatFinChallenge.Xtra;

namespace TheGreatFinChallenge.Models.Views
{
    public class SettingsView
    {
        public User CurrentUser { get; set; }
        public List<Directorate> Directorates;
        public Dictionary<Directorate, List<Department>> Departments = new Dictionary<Directorate, List<Department>>();

        public SettingsView(TGFCContext _ctx, IEnumerable<Claim> _claims)
        {
            CurrentUser = Queries.GetUserByClaims(_ctx, _claims);
            Directorates = Queries.GetAllDirectorates(_ctx);
            foreach (var directorate in Directorates) Departments[directorate] = Queries.GetAllDepartmentsOfDirectorate(_ctx, directorate);
        }

    }
}
