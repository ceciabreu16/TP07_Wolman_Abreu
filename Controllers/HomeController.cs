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
            return RedirectToAction("Usuarios");
        }
        else
        {
            ViewBag.Error = "Usuario o contraseña no válidos.";
            return View("IniciarSesion");
        }
    }
    public IActionResult Usuarios()
    {
        string usuarioLogueado = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuarioLogueado))
        {
            return RedirectToAction("Index");
        }
        ViewBag.usuario = usuarioLogueado;
        ViewBag.listaTareas = BD.ListarTareas(usuarioLogueado);
        return View();
    }
    public IActionResult NuevaTarea(string Titulo, string estado, DateTime FechaCreacion)
    {
        string usuarioLogueado = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuarioLogueado))
        {
            return RedirectToAction("Index");
        }
        if (!string.IsNullOrEmpty(Titulo) && !string.IsNullOrEmpty(estado) && FechaCreacion != default(DateTime))
        {
            var tareas = BD.ListarTareas(usuarioLogueado);
            BD.InsertarTarea(Titulo, estado, usuarioLogueado, FechaCreacion);
            return RedirectToAction("TareasLista");
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
        {
            return RedirectToAction("Usuarios");
        }
        ViewBag.tarea = tarea;
        return View();
    }

    [HttpPost]
    public IActionResult EditarTarea(int id, string Titulo, string estado, DateTime FechaCreacion)
    {
        BD.EditarTarea(id, Titulo, estado, FechaCreacion);
        return RedirectToAction("Usuarios");
    }

    [HttpPost]
    public IActionResult Registrarse(string Username, string password)
    {
        if (BD.UsuarioExiste(Username))
        {
            ViewBag.Error = "El usuario ya existe, elija otro nombre.";
            return View();
        }
        else
        {
            BD.RegistrarUsuario(Username, password);
            ViewBag.Exito = "¡Registro exitoso!";
            //ViewBag.ShowLoginLink = true;
            return View();
        }
    }
    public IActionResult CompartirTarea(int tareaId, string usuarioCompartir)
    {
        if (BD.UsuarioExiste(usuarioCompartir))
        {
            int usuarioId = BD.ObtenerIdUsuario(usuarioCompartir);

            if (!BD.TareaYaCompartidaConUsuario(tareaId, usuarioId))
            {
                BD.AgregarTareaCompartida(tareaId, usuarioId);
                ViewBag.TCompartidaUsuario = usuarioCompartir;
                ViewBag.Exito = "El usuario " + usuarioCompartir + " ya podrá ver la tarea compartida";
            }
            else
            {
                ViewBag.Error = "La tarea ya está compartida con ese usuario.";
            }
            return View();
        }
        else
        {
            ViewBag.Error = "El usuario no fue encontrado";
            return View();
        }
    }

    [HttpPost]
    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}