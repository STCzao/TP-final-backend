using System.ComponentModel.DataAnnotations;

namespace tp_final_backend.Models
{
    public class Paciente
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
        [StringLength(20)]
        public string Dni { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Turno> Turnos { get; set; } = [];
    }
}
