using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP07_Wolman_Abreu.Models;

namespace TP07_Wolman_Abreu.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
