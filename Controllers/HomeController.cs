using Microsoft.AspNetCore.Mvc;
using TPNoNum_Wolman_Abreu.Models;
using try_catch_poc.Models;
namespace TPNoNum_Wolman_Abreu.Controllers;

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
    public IActionResult IniciarSesion()
    {
        return View();
    }

    public IActionResult Registrarse()
    {
        return View();
    }
    public IActionResult Ingresar(string Username, string password)
    {
        Usuario nuevoUsuario = BD.BuscarUsuario(Username, password);
        if (nuevoUsuario != null)
        {
            HttpContext.Session.SetString("usuario", nuevoUsuario.Username);
            return RedirectToAction("Usuarios"); // 
        }
        else
        {
            ViewBag.Error = "Usuario o contraseña no válidos.";
            return View("IniciarSesion");
        }
    }
    public IActionResult Registrar(string Username, string password)
    {
        Usuario nuevoUsuario = BD.BuscarUsuario(Username, password);
        if (nuevoUsuario != null) // aca hay que comparar usuarios
        {
            ViewBag.Error = "Usuario ya existente.";
            return View("Registrarse"); // 
        }
        else
        {
            // hay que agregar insertar crear el usuario
            HttpContext.Session.SetString("usuario", nuevoUsuario.Username);
            return RedirectToAction("Usuarios");
        }
    }
    public IActionResult Usuarios()
    {
        ViewBag.usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(ViewBag.usuario))
        {
            return RedirectToAction("Index");
        }
        // Asignar una lista vacía si no venís por TareasLista
        ViewBag.listaTareas = new List<Tareas>();
        return View();
    }
    public IActionResult NuevaTarea(string Titulo, string estado)
    {
        string usuarioLogueado = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuarioLogueado))
        {
            return RedirectToAction("Index");
        }

        if (!string.IsNullOrEmpty(Titulo) && !string.IsNullOrEmpty(estado))
        {
            BD.InsertarTarea(Titulo, estado, usuarioLogueado);
            return RedirectToAction("TareasLista"); // Mostrá la lista actualizada
        }

        return View();
    }
    public IActionResult TareasLista()
    {
        string usuarioLogueado = HttpContext.Session.GetString("usuario");

        if (string.IsNullOrEmpty(usuarioLogueado))
        {
            return RedirectToAction("Index");
        }

        ViewBag.listaTareas = BD.ListarTareas(usuarioLogueado);
        ViewBag.usuario = usuarioLogueado;

        return View("Usuarios");
    }
    public IActionResult VerTarea(int id)
    {
        var tarea = BD.BuscarTareaPorId(id);
        if (tarea == null)
            return RedirectToAction("Usuarios");
        ViewBag.tarea = tarea;
        return View();
    }

    [HttpPost]
    public IActionResult EditarTarea(int id, string Titulo, string estado)
    {
        BD.EditarTarea(id, Titulo, estado);
        return RedirectToAction("Usuarios");
    }

}