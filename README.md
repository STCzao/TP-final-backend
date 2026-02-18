# Turnero Médico - Backend API

Sistema de gestión de turnos médicos desarrollado con ASP.NET Core 8.0 y PostgreSQL.

## 📋 Descripción del Proyecto

Este proyecto es una aplicación Web API desarrollada con ASP.NET Core MVC que implementa un sistema de turnero médico. Permite gestionar pacientes, doctores y turnos médicos, cumpliendo con todas las buenas prácticas de desarrollo como el patrón Repository, DTOs y AutoMapper.

## 🎯 Características

- **Gestión de Pacientes**: CRUD completo para pacientes
- **Gestión de Doctores**: CRUD completo para doctores con especialidades
- **Gestión de Turnos**: Sistema completo de turnos con validaciones de disponibilidad
- **Validaciones**: Validación de datos, horarios disponibles, y prevención de duplicados
- **Documentación API**: Swagger/OpenAPI integrado
- **Base de Datos**: PostgreSQL con Entity Framework Core
- **Arquitectura**: Patrón Repository, DTOs, AutoMapper

## 📁 Estructura del Proyecto

```
tp-final-backend/
│
├── Controllers/              # Controladores API
│   ├── PacientesController.cs
│   ├── DoctoresController.cs
│   └── TurnosController.cs
│
├── Models/                   # Modelos de entidades
│   ├── Paciente.cs
│   ├── Doctor.cs
│   └── Turno.cs
│
├── DTOs/                     # Data Transfer Objects
│   ├── PacienteDto.cs
│   ├── DoctorDto.cs
│   └── TurnoDto.cs
│
├── Data/                     # Contexto de base de datos
│   └── ApplicationDbContext.cs
│
├── Repositories/             # Patrón Repository
│   ├── Interfaces/
│   │   ├── IPacienteRepository.cs
│   │   ├── IDoctorRepository.cs
│   │   └── ITurnoRepository.cs
│   ├── PacienteRepository.cs
│   ├── DoctorRepository.cs
│   └── TurnoRepository.cs
│
├── Mappings/                 # Perfiles de AutoMapper
│   └── MappingProfile.cs
│
├── Program.cs               # Configuración de la aplicación
└── appsettings.json         # Configuración y cadenas de conexión
```

## 🛠️ Tecnologías Utilizadas

- **.NET 8.0**: Framework principal
- **ASP.NET Core Web API**: Para crear la API RESTful
- **Entity Framework Core 8.0**: ORM para acceso a datos
- **PostgreSQL**: Base de datos relacional
- **AutoMapper 12.0**: Mapeo de objetos
- **Swashbuckle (Swagger)**: Documentación de API

## 📦 Requisitos Previos

1. **.NET 8.0 SDK** instalado
2. **PostgreSQL** instalado y ejecutándose localmente
3. **Visual Studio 2022** o **Visual Studio Code**
4. **Git** (opcional)

## 🚀 Instalación y Configuración

### 1. Instalar PostgreSQL

Si no tienes PostgreSQL instalado:

- Descarga desde: https://www.postgresql.org/download/windows/
- Durante la instalación, configura:
  - Usuario: `postgres`
  - Contraseña: `postgres` (o la que prefieras)
  - Puerto: `5432` (por defecto)

### 2. Configurar la Base de Datos

Abre pgAdmin o psql y ejecuta:

```sql
CREATE DATABASE turnero_medico;
```

### 3. Configurar la Cadena de Conexión

Si configuraste una contraseña diferente en PostgreSQL, edita el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=turnero_medico;Username=postgres;Password=TU_CONTRASEÑA"
  }
}
```

### 4. Restaurar Dependencias

```bash
dotnet restore
```

### 5. Crear las Migraciones y la Base de Datos

```bash
# Instalar herramientas de EF Core (si no las tienes)
dotnet tool install --global dotnet-ef

# Crear la migración inicial
dotnet ef migrations add InitialCreate

# Aplicar la migración a la base de datos
dotnet ef database update
```

### 6. Ejecutar la Aplicación

```bash
dotnet run
```

La aplicación se ejecutará en:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

## 📊 Modelo de Datos

### Paciente
- Id (PK)
- Nombre
- Apellido
- DNI (único)
- Email
- Teléfono
- Fecha de Nacimiento
- Dirección
- Fecha de Registro

### Doctor
- Id (PK)
- Nombre
- Apellido
- Especialidad
- Matrícula (única)
- Email
- Teléfono
- Activo
- Fecha de Registro

### Turno
- Id (PK)
- PacienteId (FK)
- DoctorId (FK)
- FechaHora
- Motivo
- Estado (Pendiente, Confirmado, Cancelado, Completado)
- Observaciones
- Fecha de Creación

## 🔌 Endpoints de la API

### Pacientes

```
GET    /api/pacientes              - Obtener todos los pacientes
GET    /api/pacientes/{id}         - Obtener paciente por ID
GET    /api/pacientes/dni/{dni}    - Obtener paciente por DNI
POST   /api/pacientes              - Crear nuevo paciente
PUT    /api/pacientes/{id}         - Actualizar paciente
DELETE /api/pacientes/{id}         - Eliminar paciente
```

### Doctores

```
GET    /api/doctores                         - Obtener todos los doctores
GET    /api/doctores/activos                 - Obtener doctores activos
GET    /api/doctores/{id}                    - Obtener doctor por ID
GET    /api/doctores/matricula/{matricula}   - Obtener doctor por matrícula
GET    /api/doctores/especialidad/{esp}      - Obtener doctores por especialidad
POST   /api/doctores                         - Crear nuevo doctor
PUT    /api/doctores/{id}                    - Actualizar doctor
DELETE /api/doctores/{id}                    - Eliminar doctor
```

### Turnos

```
GET    /api/turnos                    - Obtener todos los turnos
GET    /api/turnos/{id}               - Obtener turno por ID
GET    /api/turnos/paciente/{id}      - Obtener turnos de un paciente
GET    /api/turnos/doctor/{id}        - Obtener turnos de un doctor
GET    /api/turnos/fecha/{fecha}      - Obtener turnos por fecha
GET    /api/turnos/estado/{estado}    - Obtener turnos por estado
POST   /api/turnos                    - Crear nuevo turno
PUT    /api/turnos/{id}               - Actualizar turno
PATCH  /api/turnos/{id}/estado        - Actualizar solo el estado
DELETE /api/turnos/{id}               - Eliminar turno
```

## 🧪 Probar la API

### Usando Swagger

1. Ejecuta la aplicación: `dotnet run`
2. Abre en el navegador: https://localhost:5001/swagger
3. Prueba los diferentes endpoints directamente desde la interfaz

### Ejemplo de Solicitud POST - Crear Paciente

```json
POST /api/pacientes
Content-Type: application/json

{
  "nombre": "Juan",
  "apellido": "Pérez",
  "dni": "12345678",
  "email": "juan.perez@example.com",
  "telefono": "381-4567890",
  "fechaNacimiento": "1990-01-15",
  "direccion": "Calle Principal 123"
}
```

### Ejemplo de Solicitud POST - Crear Turno

```json
POST /api/turnos
Content-Type: application/json

{
  "pacienteId": 1,
  "doctorId": 1,
  "fechaHora": "2026-02-20T10:00:00",
  "motivo": "Consulta general"
}
```

## 🎓 Buenas Prácticas Implementadas

1. **Patrón Repository**: Abstracción del acceso a datos
2. **DTOs**: Separación entre modelos de dominio y de transferencia
3. **AutoMapper**: Mapeo automático entre entidades y DTOs
4. **Validaciones**: Validación de datos en DTOs y lógica de negocio
5. **Async/Await**: Operaciones asincrónicas para mejor rendimiento
6. **Dependency Injection**: Inyección de dependencias
7. **RESTful API**: Diseño de API siguiendo principios REST
8. **Documentación**: Swagger/OpenAPI para documentación interactiva

## 🗃️ Comandos Útiles de Entity Framework

```bash
# Crear una nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones pendientes
dotnet ef database update

# Revertir a una migración anterior
dotnet ef database update NombreDeLaMigracion

# Eliminar la última migración (si no se aplicó)
dotnet ef migrations remove

# Ver el SQL que se ejecutará
dotnet ef migrations script

# Eliminar la base de datos
dotnet ef database drop
```

## 📝 Notas para la Presentación

- El proyecto cumple con todos los requisitos del trabajo práctico final
- Implementa un CRUD completo para tres entidades relacionadas
- Utiliza Entity Framework Core conectado a PostgreSQL
- Sigue buenas prácticas: Repository, DTO, Mapper
- Incluye validaciones de negocio (turnos disponibles, datos únicos)
- Documentación completa con Swagger

## 👥 Autor

Trabajo Práctico Final - Desarrollo de Back End 2025
Universidad del Norte Santo Tomás de Aquino (UNSTA)

## 📄 Licencia

Este proyecto es de uso educativo para el curso de Desarrollo de Back End 2025.
