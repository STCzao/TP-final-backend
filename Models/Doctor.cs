using System.ComponentModel.DataAnnotations;

namespace tp_final_backend.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Matricula { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
