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

        ViewBag.listaTareas = BD.ListarTareas(usuario);
        ViewBag.usuario = usuario;
        return View();
    }

    public IActionResult Nueva()
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        return View();
    }

    [HttpPost]
    public IActionResult Nueva(string Titulo, string estado, DateTime FechaCreacion)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        BD.InsertarTarea(Titulo, estado, usuario, FechaCreacion);
        return RedirectToAction("Lista");
    }

    public IActionResult Editar(int id)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BD.BuscarTareaPorId(id);
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

        var tarea = BD.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        BD.EditarTarea(id, Titulo, estado, FechaCreacion);
        return RedirectToAction("Lista");
    }

    public IActionResult Eliminar(int id)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BD.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        BD.EliminarTarea(id); // Borrado lógico en la BD
        return RedirectToAction("Lista");
    }

    public IActionResult Finalizar(int id)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BD.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        BD.FinalizarTarea(id); // Marcar como finalizada en la BD
        return RedirectToAction("Lista");
    }

    public IActionResult Compartir(int id)
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario))
            return RedirectToAction("Login", "Auth");

        var tarea = BD.BuscarTareaPorId(id);
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

        var tarea = BD.BuscarTareaPorId(id);
        if (tarea == null || !tarea.EsPropia)
            return RedirectToAction("Lista");

        if (BD.UsuarioExiste(usuarioCompartir))
        {
            int usuarioId = BD.ObtenerIdUsuario(usuarioCompartir);
            if (!BD.TareaYaCompartidaConUsuario(id, usuarioId))
            {
                BD.AgregarTareaCompartida(id, usuarioId);
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