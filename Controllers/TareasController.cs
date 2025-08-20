using Microsoft.AspNetCore.Mvc;
using TPNoNum_Wolman_Abreu.Models;
using try_catch_poc.Models;
namespace TPNoNum_Wolman_Abreu.Controllers;

public class TareasController : Controller
{
    public IActionResult Lista()
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        ViewBag.listaTareas = BDTareas.ListarTareas(usuario);
        ViewBag.usuario = usuario;
        return View();
    }

    public IActionResult NuevaTarea()
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        return View();
    }

    [HttpPost]
    public IActionResult NuevaTarea(string Titulo, string estado, DateTime FechaCreacion)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        BDTareas.InsertarTarea(Titulo, estado, usuario, FechaCreacion);
        return RedirectToAction("Lista");
    }

    public IActionResult Editar(int id)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BDTareas.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        ViewBag.tarea = tarea;
        return View();
    }

    [HttpPost]
    public IActionResult Editar(int id, string Titulo, string estado, DateTime FechaCreacion)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BDTareas.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        BDTareas.EditarTarea(id, Titulo, estado, FechaCreacion);
        return RedirectToAction("Lista");
    }

    public IActionResult Eliminar(int id)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BDTareas.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        BDTareas.EliminarTarea(id); // Borrado lógico en la BD
        return RedirectToAction("Lista");
    }

    public IActionResult Finalizar(int id)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BDTareas.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        BDTareas.FinalizarTarea(id); // Marcar como finalizada en la BD
        return RedirectToAction("Lista");
    }

    public IActionResult Compartir(int id)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BDTareas.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        ViewBag.tarea = tarea;
        return View();
    }

    [HttpPost]
    public IActionResult Compartir(int id, string usuarioCompartir)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BDTareas.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        if (BDAuth.UsuarioExiste(usuarioCompartir))
        {
            int usuarioId = BDAuth.ObtenerIdUsuario(usuarioCompartir);
            if (!BDTareas.TareaYaCompartidaConUsuario(id, usuarioId))
            {
                BDTareas.AgregarTareaCompartida(id, usuarioId);
                ViewBag.Exito = $"La tarea fue compartida con {usuarioCompartir}.";
            }
            else
            {
                ViewBag.Error = "La tarea ya está compartida con ese usuario.";
            }
        }
        else
        {
            ViewBag.Error = "El usuario no existe.";
        }
        ViewBag.tarea = tarea;
        return View();
    }
}