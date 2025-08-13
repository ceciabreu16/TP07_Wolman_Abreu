namespace try_catch_poc.Models
{ 
public class Tareas
{
    public int Id { get; set; } 
    public  string Titulo { get; set; }
    public  DateTime FechaCreacion { get; set; }
    public bool EstaActiva {get; set;}
    public  bool EstaFinalizada { get; set; }
    public  bool EstaEliminada { get; set; }
    
}
}
