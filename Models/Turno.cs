using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp_final_backend.Models
{
    public class Turno
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PacienteId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [StringLength(500)]
        public string? Motivo { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmado, Cancelado, Completado

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("PacienteId")]
        public Paciente Paciente { get; set; } = null!;

        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; } = null!;
    }
}
