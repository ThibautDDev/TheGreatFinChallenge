using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheGreatFinChallenge.Models.Data;
using TheGreatFinChallenge.Xtra;

namespace TheGreatFinChallenge.Models.Views
{
    public class CreateActivityView
    {
        public User CurrentUser { get; set; }
        public List<Discipline> Disciplines { get; set; }
        public List<ActivityType> ActivityTypes { get; set; }

        public CreateActivityView(TGFCContext _ctx, IEnumerable<Claim> _claims)
        {
            CurrentUser = Queries.GetUserByClaims(_ctx, _claims);
            Disciplines = Queries.GetAllDisciplines(_ctx);
            ActivityTypes = Queries.GetAllActivityTypes(_ctx);
        }
    }
}
