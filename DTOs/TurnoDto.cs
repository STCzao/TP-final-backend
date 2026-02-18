using System.ComponentModel.DataAnnotations;

namespace tp_final_backend.DTOs
{
    public class TurnoDto
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorNombre { get; set; } = string.Empty;
        public string DoctorEspecialidad { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string? Motivo { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class CreateTurnoDto
    {
        [Required(ErrorMessage = "El paciente es requerido")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El doctor es requerido")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "La fecha y hora son requeridas")]
        public DateTime FechaHora { get; set; }

        [StringLength(500)]
        public string? Motivo { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }

    public class UpdateTurnoDto
    {
        [Required(ErrorMessage = "El paciente es requerido")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El doctor es requerido")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "La fecha y hora son requeridas")]
        public DateTime FechaHora { get; set; }

        [StringLength(500)]
        public string? Motivo { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
