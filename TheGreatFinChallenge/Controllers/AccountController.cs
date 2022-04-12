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

        public async Task<IActionResult> InitData(string returnUrl)
        {
            User u = Queries.GetUserByClaims(_context, User.Claims);
            if (u.Admin)
            {
                var www = _environment.WebRootPath;

                _context.Database.ExecuteSqlRaw($"" +
                        $"INSERT INTO [Discipline]([Name], [NameNormalized], [Color], ImageData, IconData) " +
                        $@"VALUES " +
                        $@"('Cycling', 'cycling', '#800000', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\cycling.png', SINGLE_BLOB) as img), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\cycling-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Hiking', 'hiking', '#fabed4', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\hiking.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\hiking-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Running', 'running', '#808000', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\running.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\running-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Swimming', 'swimming', '#469990', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\swimming.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\swimming-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Ball Sports', 'ballSports', '#000075', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\ball.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\ball-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Racket Sports', 'racketSports', '#e6194B', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\racket.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\racket-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Martial Arts', 'martialArts', '#ffe119', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\combat.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\combat-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Fitness', 'fitness', '#bfef45', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\fitness.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\fitness-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Winter Sports', 'winterSports', '#42d4f4', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\winter.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\winter-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Summer Sports', 'summerSports', '#911eb4', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\summer.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\summer-solid.svg', SINGLE_BLOB) as T1))," +
                        $@"('Horse Riding', 'horseRiding', '#f032e6', (SELECT * FROM OPENROWSET(BULK N'{www}\img\disciplines\horse.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'{www}\img\icons\horse-solid.svg', SINGLE_BLOB) as T1));");

                _context.Database.ExecuteSqlRaw($"" +
                        $@"INSERT INTO [ActivityType]([DisciplineId], [Name], MET, ImageData) " +
                        $@"VALUES " +
                        $@"(1, 'Analogue bike - 16 to 19 km/h', 7, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\bike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'Analogue bike - 19 to 22 km/h', 8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\bike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'Analogue bike - 22 to 25 km/h', 10, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\bike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'Analogue bike - 25 to 30 km/h', 12, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\bike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'Analogue bike - 30+ km/h', 16, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\bike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'E-bike - 16 to 19 km/h', 3.5, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\ebike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'E-bike - 19 to 22 km/h', 4, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\ebike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'E-bike - 22 to 25 km/h', 5, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\ebike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'E-Bike - 25 to 30 km/h', 6, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\ebike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'E-Bike - 30+ km/h', 8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\ebike.png', SINGLE_BLOB) as img))," +
                        $@"(1, 'Mountainbike - Offroad', 9, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\mbike.png', SINGLE_BLOB) as img))," +
                        $@"(2, 'Hiking', 4.3, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\hiking.png', SINGLE_BLOB) as img))," +
                        $@"(3, '0 to 6 km/h', 6, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\running.png', SINGLE_BLOB) as img))," +
                        $@"(3, '6 to 8 km/h', 8.3, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\running.png', SINGLE_BLOB) as img))," +
                        $@"(3, '8 to 10 km/h', 9.8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\running.png', SINGLE_BLOB) as img))," +
                        $@"(3, '10 to 11 km/h', 11, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\running.png', SINGLE_BLOB) as img))," +
                        $@"(3, '11 to 13 km/h', 11.8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\running.png', SINGLE_BLOB) as img))," +
                        $@"(3, '13 to 14 km/h', 12.8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\running.png', SINGLE_BLOB) as img))," +
                        $@"(3, '14 to 17 km/h', 14.5, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\running.png', SINGLE_BLOB) as img))," +
                        $@"(3, '17 to 19 km/h', 16, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\running.png', SINGLE_BLOB) as img))," +
                        $@"(3, '19+ km/h', 19, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\running.png', SINGLE_BLOB) as img))," +
                        $@"(4, 'Swimming', 6.8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\swimming.png', SINGLE_BLOB) as img))," +
                        $@"(5, 'Basketball', 6.5, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\basketball.png', SINGLE_BLOB) as img))," +
                        $@"(5, 'Golf', 4.8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\golf.png', SINGLE_BLOB) as img))," +
                        $@"(5, 'Handball', 8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\handball.png', SINGLE_BLOB) as img))," +
                        $@"(5, 'Field Hockey', 7.8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\hockey.png', SINGLE_BLOB) as img))," +
                        $@"(5, 'Football', 6.3, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\football.png', SINGLE_BLOB) as img))," +
                        $@"(5, 'Soccer', 7, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\soccer.png', SINGLE_BLOB) as img))," +
                        $@"(5, 'Volley-ball', 4, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\volley-ball.png', SINGLE_BLOB) as img))," +
                        $@"(6, 'Badminton', 5.5, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\badminton.png', SINGLE_BLOB) as img))," +
                        $@"(6, 'Padel', 5.3, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\padel.png', SINGLE_BLOB) as img))," +
                        $@"(6, 'Squash', 7.3, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\squash.png', SINGLE_BLOB) as img))," +
                        $@"(6, 'Table Tennis', 4, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\ttennis.png', SINGLE_BLOB) as img))," +
                        $@"(6, 'Tennis', 7.3, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\tennis.png', SINGLE_BLOB) as img))," +
                        $@"(7, 'Martial Arts', 5.3, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\martialarts.png', SINGLE_BLOB) as img))," +
                        $@"(8, 'Cardio - Strength training', 7.8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\cardio.png', SINGLE_BLOB) as img))," +
                        $@"(8, 'Dancing - Aerobics', 7.3, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\aerobics.png', SINGLE_BLOB) as img))," +
                        $@"(8, 'Workout (General)', 6, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\workout.png', SINGLE_BLOB) as img))," +
                        $@"(9, 'Skiing - Snowboarding', 7, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\skiing.png', SINGLE_BLOB) as img))," +
                        $@"(9, 'Ice skating', 5.5, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\iceskating.png', SINGLE_BLOB) as img))," +
                        $@"(10, 'Roller skating', 7, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\rskating.png', SINGLE_BLOB) as img))," +
                        $@"(10, 'Inline skating', 9.8, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\iskating.png', SINGLE_BLOB) as img))," +
                        $@"(10, 'surfing - Sailing', 3.3, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\surfing.png', SINGLE_BLOB) as img))," +
                        $@"(11, 'Horse riding', 5.5, (SELECT * FROM OPENROWSET(BULK N'{www}\img\activityTypes\horse.png', SINGLE_BLOB) as img));");




                TempData["Succes"] = "The data is succesfully initialized.";
                return RedirectToActionPermanent(returnUrl.Split("_")[1], returnUrl.Split("_")[0]);
            }
            TempData["Error"] = "Invalid rights. Please contact an administrator.";
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
