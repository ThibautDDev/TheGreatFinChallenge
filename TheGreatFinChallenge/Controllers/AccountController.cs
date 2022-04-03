using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheGreatFinChallenge.Models;
using TheGreatFinChallenge.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using TheGreatFinChallenge.Models.Views;
using TheGreatFinChallenge.Xtra;
using System.Globalization;

namespace TheGreatFinChallenge.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly TGFCContext _context;
        private IHostingEnvironment _environment;
        private Dictionary<string, string> UserDictionairy = new Dictionary<string, string>();


        public AccountController(ILogger<AccountController> logger, TGFCContext context, IHostingEnvironment environment)
        {
            _logger = logger;
            _context = context;
            _environment = environment;
        }


        public IActionResult Index() => View(new AccountView(_context, User.Claims));
        public IActionResult Settings() => View(new SettingsView(_context, User.Claims));
        public IActionResult Activities(string ActivityId) => View(new ActivitiesView(_context, User.Claims));
        public IActionResult CreateActivity() => View(new CreateActivityView(_context, User.Claims));
        public IActionResult Departments() => View(new DepartmentsView(_context, User.Claims));
        public IActionResult Images() => View(new ImagesView(_context, User.Claims));


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("~/");
        }

        public async Task<IActionResult> UpdateSettings(string firstName, string lastName, string email,
            int departmentId, string oldPassword, string password, string passwordConfirmed,
            bool gdpr, string returnUrl)
        {
            User u = Queries.GetUserByClaims(_context, User.Claims);
            if (password != "" && password != null && Hash.HashPassword(oldPassword, u.Salt) == u.Password && password == passwordConfirmed)
            {
                u.FirstName = firstName;
                u.LastName = lastName;
                u.Email = email;
                u.DepartmentId = departmentId;
                u.Gdpr = gdpr;
                Byte[] salt = Hash.GenerateSalt();
                var PasswordEncrypted = Hash.HashPassword(password, salt);
                u.Salt = Hash.ConvertSaltToString(salt);
                u.Password = PasswordEncrypted;
                _context.User.Update(u);
                await _context.SaveChangesAsync();
                TempData["Succes"] = "The settings are succesfully saved.";
                return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
            }
            else if (password == "" || password == null)
            {
                u.FirstName = firstName;
                u.LastName = lastName;
                u.Email = email;
                u.DepartmentId = departmentId;
                u.Gdpr = gdpr;
                _context.User.Update(u);
                await _context.SaveChangesAsync();
                TempData["Succes"] = "The settings are succesfully saved.";
                return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
            }
            else if (Hash.HashPassword(oldPassword, u.Salt) != u.Password) TempData["PasswordError"] = "Old Password did not match with this account.";
            else if (password != passwordConfirmed && password != "") TempData["NewPasswordError"] = "New password did not match with its confirmation.";
            else if (!Hash.PasswordMeetsRequirements(password)) TempData["PasswordRequirementsError"] = "Please make sure that the new password meets the requirements.";
            else TempData["Error"] = "Something went wrong. Please contact an administrator.";

            return RedirectToActionPermanent("Settings", "Account");
        }

        public async Task<IActionResult> UpdateChallengeDates(string returnUrl, int directorateId, DateTime startDate, DateTime endDate)
        {
            try
            {
                Directorate d = Queries.GetDirectorateById(_context, directorateId);
                d.ChallengeStartDate = startDate;
                d.ChallengeEndDate = endDate;
                _context.Directorate.Update(d);
                await _context.SaveChangesAsync();

                TempData["Succes"] = "The settings are succesfully saved.";
                return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
            }
            catch
            {
                TempData["Error"] = "Something went wrong. Please contact an administrator";
                return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
            }
        }

        public async Task<IActionResult> ResetPasswordOfUser(string returnUrl, string email, string firstName)
        {
            try
            {
                User u = Queries.GetUserByEmail(_context, email);
                if (u.FirstName.ToLower() == firstName.ToLower())
                {
                    string password = $"@{u.FirstName}@{DateTime.Now.Year}_{new Random().Next()}";
                    u.Password = Hash.HashPassword(password, u.Salt);
                    _context.User.Update(u);
                    await _context.SaveChangesAsync();
                    TempData["Succes"] = $"Password for the user '{u.Email}' is succesfully reset. The new password is: {password}";
                }
                else
                {
                    TempData["Error"] = $"Something went wrong. Please contact an administrator";
                }
                return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
            }
            catch
            {
                TempData["Error"] = "Something went wrong. Please contact an administrator";
                return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
            }
        }

        public async Task<IActionResult> CreateAndValidateActivity(string returnUrl, int disciplineId, int activityTypeId, string distance, DateTime startDate, DateTime endDate)
        {
            string distanceNormalized = distance.Replace(',', '.');
            double dDistance;
            double.TryParse(distanceNormalized, NumberStyles.Any, CultureInfo.InvariantCulture, out dDistance);

            User currentUser = Queries.GetUserByClaims(_context, User.Claims);
            Activity a = new Activity(currentUser.UserId, activityTypeId, dDistance, startDate, endDate);
            await _context.Activity.AddAsync(a);
            await _context.SaveChangesAsync();

            return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
        }

        public async Task<IActionResult> EditActivity(string returnUrl, int activityId, int activityTypeId, string distance, DateTime startDate, DateTime endDate)
        {
            string distanceNormalized = distance.Replace(',', '.');
            double dDistance;
            double.TryParse(distanceNormalized, NumberStyles.Any, CultureInfo.InvariantCulture, out dDistance);

            Activity a = Queries.GetActivityById(_context, activityId);
            a.ActivityTypeId = activityTypeId;
            a.Distance = dDistance;
            a.StartTime = startDate;
            a.EndTime = Activity.WithDate(endDate, startDate);

            _context.Activity.Update(a);
            await _context.SaveChangesAsync();

            return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
        }

        public async Task<IActionResult> DeleteActivity(string returnUrl, int activityId)
        {
            Activity a = Queries.GetActivityById(_context, activityId);
            _context.Activity.Remove(a);
            await _context.SaveChangesAsync();

            return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
        }

        public async Task<IActionResult> UploadImage(string returnUrl, int userId, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                Byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                Image i = new Image(userId, fileBytes);
                _context.Image.Add(i);
                await _context.SaveChangesAsync();

                TempData["Succes"] = "The image is succesfully uploaded.";
            }
            else
            {
                TempData["Error"] = "Something went wrong while progressing the image. Please contact an administrator.";
            }
            return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
        }

        public async Task<IActionResult> DeleteImage(string returnUrl, int imageId)
        {
            try
            {
                Image i = Queries.getImageById(_context, imageId);

                _context.Image.Remove(i);
                await _context.SaveChangesAsync();
                TempData["Succes"] = "The image is succesfully uploaded.";
            }
            catch
            {
                TempData["Error"] = "Something went wrong while deleting the image. Please contact an administrator.";
            }
            return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
        }

        //    // POST: /Backup
        //    [HttpPost("Backup")]
        //    public async Task<ActionResult> GetBackupFile()
        //    {
        //        setUserDictionairy();
        //        var id = Convert.ToInt32(UserDictionairy["UserID"]);
        //        var user = await getUserByIdAsync(_context, id);

        //        if (user.Admin)
        //        {
        //            string webRootPath = _environment.WebRootPath;
        //            string date = DateTime.Now.ToString("dd_MM_yyyy");
        //            string fileName = String.Format("Backup_{0}.txt", date);
        //            string path = Path.Combine(webRootPath, "BackupTXT\\", fileName);

        //            if (System.IO.File.Exists(path))
        //            {
        //                System.IO.File.Delete(path);
        //            }


        //            var data = "USE TheGreatFinChallenge\nGO\n\n";
        //            //Users
        //            data += "SET IDENTITY_INSERT Users ON\n";
        //            data += "GO\n";
        //            data += "INSERT INTO Users (pk_UserID, FirstName, LastName, [Admin], Email, PasswordHash, [Hash], CreationDate)\nVALUES\n";
        //            List<User> users = await getAllUsersByIdAsync(_context);
        //            foreach (var u in users)
        //            {
        //                var line = $"({u.pk_UserID}, '{u.FirstName}', '{u.LastName}', {Convert.ToInt32(u.Admin)},'{u.Email}', '{u.PasswordHash}', '{u.Hash}', '{u.CreationDate.ToString("yyyy-MM-dd")}')";
        //                if (u == users.Last()) line += ";\n";
        //                else line += ",\n";
        //                data += line;
        //            }
        //            data += "SET IDENTITY_INSERT Users ON\n";
        //            data += "GO\n\n";


        //            //Groups
        //            data += "INSERT INTO Groups (fk_CreatorID, GroupName)\nVALUES\n";
        //            List<Groups> groups = await getAllGroupsAsync(_context);
        //            foreach (var g in groups)
        //            {
        //                var line = $"({g.fk_CreatorID}, '{g.GroupName}')";
        //                if (g == groups.Last()) line += ";\n\n";
        //                else line += ",\n";
        //                data += line;
        //            }


        //            //GroupMemberships
        //            data += "INSERT INTO GroupMembership(UserID, GroupID)\nVALUES\n";
        //            List<GroupMembership> groupmemberships = await getAllGroupMembershipsAsync(_context);
        //            foreach (var gm in groupmemberships)
        //            {
        //                var line = $"({gm.UserID}, {gm.GroupID})";
        //                if (gm == groupmemberships.Last()) line += ";\n\n";
        //                else line += ",\n";
        //                data += line;
        //            }


        //            //Activities
        //            data += "INSERT INTO Activities (fk_UserID, ActivityType, TotalCalories, Distance, TTime, StartTime, Gear)\nVALUES\n";
        //            List<Activities> activities = await getAllActivitiesAsync(_context);
        //            foreach (var a in activities)
        //            {
        //                var line = $"({a.fk_UserID}, '{a.ActivityType}', {a.TotalCalories}, {Convert.ToString(a.Distance).Replace(",", ".")}, '{a.TTime}', '{a.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}', '{a.Gear}')";
        //                if (a == activities.Last()) line += ";";
        //                else line += ",\n";
        //                data += line;
        //            }

        //            using (StreamWriter outputFile = new StreamWriter(path))
        //            {
        //                await outputFile.WriteAsync(data);
        //            }

        //            return PhysicalFile(path, "application/octet-stream", fileName);
        //        } 
        //        else
        //        {
        //            TempData["Error"] = "You don't have the permissions to do this. Please contact an administrator.";
        //            return Redirect("~/Account/Settings");
        //        }

        //    }
    }
}
