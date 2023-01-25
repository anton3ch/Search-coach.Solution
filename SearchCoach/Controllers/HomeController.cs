using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SearchCoach.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System;

namespace SearchCoach.Controllers
{
  public class HomeController : Controller
  {
    private readonly SearchCoachContext _db;
    // private readonly UserManager<ApplicationUser> _userManager;

     public HomeController(SearchCoachContext db)
      {
        _db = db;
      }
    public ActionResult Index()
    {
      // 
      // Stats!
      Dictionary<string, int> stats = new Dictionary<string, int>();

      // WeeklyAppAvg = total app count / total weeks
      // get total app count
      int AppCount = _db.Applications.Count();
      double AppCountToDouble = System.Convert.ToDouble(AppCount); // get count of all apps 1
      // get total days count
      double dateNow = DateTime.Now.ToOADate(); // get current date 1/24/22
      Application FirstApp = _db.Applications.FirstOrDefault(model => model.ApplicationId == 1); // get date of first app ever submitted 1/23/22
      double dateOfFirstApp = FirstApp.Date.ToOADate(); // get date of first app ever submitted 1/23/22
      int elapsedDays = (int)(dateNow - dateOfFirstApp); // get elapsed days
      int elapsedWeeks = (elapsedDays / 7); if ((elapsedDays % 7) > 0) {elapsedWeeks++;} // get elapsed weeks
      // TODO: add test for above
        // if elapsedDays == 0, then 0 wk
        // if elapsedDays == 2, then 1 wk
        // if elapsedDays == 21, then 3 wk
        // if elapsedDays == 23, then 4 wk
      double WeeklyAppAvgDouble = (AppCountToDouble / elapsedWeeks);
      int WeeklyAppAvg = (int)WeeklyAppAvgDouble;

      stats.Add("WeeklyAppAvg", WeeklyAppAvg);

      // AllTimeAppCount = total app count
      int AllTimeAppCount = _db.Applications.Count();

      stats.Add("AllTimeAppCount", AllTimeAppCount);

      // AllTimeCompCount = total comp count
      int AllTimeCompCount = _db.Companies.Count();

      stats.Add("AllTimeCompCount", AllTimeCompCount);

      // AllTimePhoneCount = total phone screen count
      int AllTimePhoneScreen = _db.Applications
                                  .Include(model => model.Status)
                                  .Where(model => model.Status.PhoneScreen == true).Count();
                                  
      stats.Add("AllTimePhoneScreen", AllTimePhoneScreen);

      // AllTimeInterview = total interview count

      int AllTimeInterview1  = _db.Applications
                                  .Include(model => model.Status)
                                  .Where(model => model.Status.Interview1 == true).Count();
      int AllTimeInterview2  = _db.Applications
                                  .Include(model => model.Status)
                                  .Where(model => model.Status.Interview2 == true).Count();
                                
      int AllTimeInterview = AllTimeInterview1 + AllTimeInterview2;
      stats.Add("AllTimeInterview", AllTimeInterview);

      Company[] companies = _db.Companies.ToArray();
      Application[] applications = _db.Applications.Include(model => model.Status).ToArray();
      Dictionary<string, object[]> model = new Dictionary<string, object[]>();
      model.Add("companies", companies);
      model.Add("applications", applications);
      ViewBag.Stats = stats;
      return View(model);
    }

    public ActionResult Search(string query)
    {
      Dictionary<string,object[]> model = new Dictionary<string, object[]>();
      Application[] applications = _db.Applications.Where(application => application.Role.Contains(query)).ToArray();
      model.Add("applications", applications);
      Company[] companies = _db.Companies.Where(company => company.Name.Contains(query)).ToArray();
      model.Add("companies", companies);
      return View(model);
    }

  }
}