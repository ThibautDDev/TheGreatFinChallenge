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
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;


namespace TheGreatFinChallenge.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Uh oh, this page could not be found";
                    ViewBag.ErrorNumber = "404";
                    ViewBag.ErrorLongMessage = "The page you are looking for might have been removed, had its name changed or is temporarily unavailable";
                    break;
                case 405:
                    ViewBag.ErrorMessage = "Method not allowed";
                    ViewBag.ErrorNumber = "405";
                    ViewBag.ErrorLongMessage = "A request method is not supported for the requested resource. Please contact an administrator";
                    break;
                case 401:
                    ViewBag.ErrorMessage = "Not Authorized";
                    ViewBag.ErrorNumber = "401";
                    ViewBag.ErrorLongMessage = "You do not have acccess to this page. Please make sure you are logged in, or contact your administrator";
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Internal Server Error";
                    ViewBag.ErrorNumber = "500";
                    ViewBag.ErrorLongMessage = "The server encountered an internal error or misconfiguration and was unable to complete your request. Please contact an administrator";
                    break;
                case 403:
                    ViewBag.ErrorMessage = "Forbidden";
                    ViewBag.ErrorNumber = "403";
                    ViewBag.ErrorLongMessage = "The page or resource you were trying to reach is absolutely forbidden for some reason";
                    break;
                case 503:
                    ViewBag.ErrorMessage = "Service unavailable";
                    ViewBag.ErrorNumber = "503";
                    ViewBag.ErrorLongMessage = "The server encounterd an internal struggle to handle your request. Try again later or contact an administrator";
                    break;
                case 504:
                    ViewBag.ErrorMessage = "Gateway Timeout";
                    ViewBag.ErrorNumber = "504";
                    ViewBag.ErrorLongMessage = "The page is taking way too long to load. Please try to refresh or contact an administrator";
                    break;
                default:
                    ViewBag.ErrorMessage = "Error";
                    ViewBag.ErrorNumber = statusCode;
                    ViewBag.ErrorLongMessage = "We having some troubles on our side. Please contact an administrator";
                    break;
            }
            return View("Error");
        }
    }
}