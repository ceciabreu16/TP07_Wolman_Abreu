namespace try_catch_poc.Models
{
    public class Tareas
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool EstaFinalizada { get; set; }
        public bool EstaEliminada { get; set; }
        public bool EsPropia { get; set; }
    }
    public class TareasCompartidas
    {
        public int Id { get; set; }
        public int TareaId { get; set; }
        public int UsuarioId { get; set; }
        public bool UsuarioEsCreador { get; set; }
    }
}