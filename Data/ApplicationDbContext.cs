using Microsoft.EntityFrameworkCore;
using tp_final_backend.Models;

namespace tp_final_backend.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Doctor> Doctores { get; set; }
        public DbSet<Turno> Turnos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relaciones
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Paciente)
                .WithMany(p => p.Turnos)
                .HasForeignKey(t => t.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Doctor)
                .WithMany(d => d.Turnos)
                .HasForeignKey(t => t.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar índices únicos
            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.Dni)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Matricula)
                .IsUnique();

            // Configurar valores por defecto
            modelBuilder.Entity<Paciente>()
                .Property(p => p.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Doctor>()
                .Property(d => d.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Turno>()
                .Property(t => t.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Seed data inicial
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Especialidad = "Cardiología",
                    Matricula = "MP12345",
                    Email = "juan.perez@hospital.com",
                    Telefono = "381-4567890",
                    Activo = true,
                    FechaRegistro = DateTime.UtcNow
                },
                new Doctor
                {
                    Id = 2,
                    Nombre = "María",
                    Apellido = "González",
                    Especialidad = "Pediatría",
                    Matricula = "MP12346",
                    Email = "maria.gonzalez@hospital.com",
                    Telefono = "381-4567891",
                    Activo = true,
                    FechaRegistro = DateTime.UtcNow
                }
            );
        }
    }
}
