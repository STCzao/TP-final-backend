using System.ComponentModel.DataAnnotations;

namespace tp_final_backend.DTOs
{
    public class PacienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class CreatePacienteDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es requerido")]
        [StringLength(20, ErrorMessage = "El DNI no puede exceder 20 caracteres")]
        public string Dni { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Teléfono inválido")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime FechaNacimiento { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }
    }

    public class UpdatePacienteDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es requerido")]
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
    }
}
