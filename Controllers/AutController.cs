using Microsoft.AspNetCore.Mvc;
using TPNoNum_Wolman_Abreu.Models;
using try_catch_poc.Models;
namespace TPNoNum_Wolman_Abreu.Controllers;

public class AuthController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var usuario = BDAuth.BuscarUsuario(username, password);
        if (usuario != null)
        {
            HttpContext.Session.SetString("usuario", username);
            return RedirectToAction("Lista", "Tareas");
        }
        ViewBag.Error = "Usuario o contraseña incorrectos";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password)
    {
        if (BDAuth.UsuarioExiste(username))
        {
            ViewBag.Error = "Ese usuario ya existe";
            return View();
        }
        BDAuth.RegistrarUsuario(username, password);
        HttpContext.Session.SetString("usuario", username);
        return RedirectToAction("Lista", "Tareas");
    }
}