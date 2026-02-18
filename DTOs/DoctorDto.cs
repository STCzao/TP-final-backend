using System.ComponentModel.DataAnnotations;

namespace tp_final_backend.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class CreateDoctorDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es requerida")]
        [StringLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula es requerida")]
        [StringLength(20)]
        public string Matricula { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Telefono { get; set; }
    }

    public class UpdateDoctorDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es requerida")]
        [StringLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula es requerida")]
        [StringLength(20)]
        public string Matricula { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Telefono { get; set; }

        public bool Activo { get; set; }
    }
}
