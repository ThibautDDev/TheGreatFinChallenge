using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheGreatFinChallenge.Models.Data;
using TheGreatFinChallenge.Xtra;

namespace TheGreatFinChallenge.Models.Views
{
    public class RegisterView
    {
        public List<Directorate> Directorates;
        public Dictionary<Directorate, List<Department>> Departments = new Dictionary<Directorate, List<Department>>();

        public RegisterView(TGFCContext _ctx)
        {
            Directorates = Queries.GetAllDirectorates(_ctx);
            foreach (var directorate in Directorates) Departments[directorate] = Queries.GetAllDepartmentsOfDirectorate(_ctx, directorate);
        }
    }
}
