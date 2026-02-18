using AutoMapper;
using tp_final_backend.DTOs;
using tp_final_backend.Models;

namespace tp_final_backend.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Paciente mappings
            CreateMap<Paciente, PacienteDto>();
            CreateMap<CreatePacienteDto, Paciente>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
                .ForMember(dest => dest.Turnos, opt => opt.Ignore());
            CreateMap<UpdatePacienteDto, Paciente>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
                .ForMember(dest => dest.Turnos, opt => opt.Ignore());
            CreateMap<Paciente, UpdatePacienteDto>(); // Mapeo inverso para Edit

            // Doctor mappings
            CreateMap<Doctor, DoctorDto>();
            CreateMap<CreateDoctorDto, Doctor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
                .ForMember(dest => dest.Turnos, opt => opt.Ignore());
            CreateMap<UpdateDoctorDto, Doctor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
                .ForMember(dest => dest.Turnos, opt => opt.Ignore());
            CreateMap<Doctor, UpdateDoctorDto>(); // Mapeo inverso para Edit

            // Turno mappings
            CreateMap<Turno, TurnoDto>()
                .ForMember(dest => dest.PacienteNombre, 
                    opt => opt.MapFrom(src => $"{src.Paciente.Nombre} {src.Paciente.Apellido}"))
                .ForMember(dest => dest.DoctorNombre, 
                    opt => opt.MapFrom(src => $"{src.Doctor.Nombre} {src.Doctor.Apellido}"))
                .ForMember(dest => dest.DoctorEspecialidad, 
                    opt => opt.MapFrom(src => src.Doctor.Especialidad));
            
            CreateMap<CreateTurnoDto, Turno>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => "Pendiente"))
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Paciente, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());
            
            CreateMap<UpdateTurnoDto, Turno>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Paciente, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());
            CreateMap<Turno, UpdateTurnoDto>(); // Mapeo inverso para Edit
        }
    }
}
