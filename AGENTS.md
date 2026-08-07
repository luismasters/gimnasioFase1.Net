# AGENTS.md

## Descripción del proyecto
Aplicación web de gestión de gimnasio "AURA FITNESS CLUB" (AppGimn). Sistema de roles (Admin, Recepcionista, Entrenador/Instructor y Cliente) con paneles adaptativos, terminal de check-in por DNI, cobro de cuotas, padrón de socios, membresías y evaluaciones.

## Stack tecnológico
- **Framework:** ASP.NET Core MVC, .NET 8 (`net8.0`) — SDK `Microsoft.NET.Sdk.Web`
- **Base de datos:** SQL Server **LocalDB** (`Server=(localdb)\\mssqllocaldb`) con Entity Framework Core
- **Auth:** ASP.NET Core Identity (tablas AspNetUsers, login en `AccountController`)
- **IU:** Razor views + Bootstrap 5 + Bootstrap Icons + hoja `wwwroot/css/site.css` (diseño "luxury purple")

## Ubicación y estructura
- El proyecto está dentro de la carpeta **`AppGimn/`** (no en la raíz del repo).
- Carpetas clave:
  - `Controllers/` — `DashboardController` (paneles por rol), `ClienteController`, `EmpleadoController`, `AccountController`, `BaseController`
  - `Models/` — `Cliente`, `Empleado`, `Pago`, `Membresia`, `Asistencia`, `Rutina`, `EvaluacionFisica`, `Usuario`, ViewModels
  - `Data/` — `ApplicationDbContext`, `DbSeeder` (seed idempotente), `Migrations/`
  - `Views/` — `Dashboard/`, `Cliente/`, `Empleado/`, `Account/`, `Home/`, `Shared/` (partials, `_Layout.cshtml`)
  - `Helpers/`, `Properties/`, `Areas/`

## Comandos de verificación (IMPORTANTE)
Usar esta build custom para evitar el error de bloqueo de `.dll` de IIS Express:
```
dotnet build -p:OutputPath=obj\buildcheck\net8.0\ -o obj\buildcheckout\net8.0\
```
Verificar "0 Errores". Complementar con ComboBox de errores:
```
dotnet build -p:OutputPath=obj\buildcheck\net8.0\ -o obj\buildcheckout\net8.0\ 2>&1 | Select-String -Pattern "error"
```
La solución: `AppGimn.sln`. Ejecutar desde `AppGimn/`.

## Convenciones y reglas del proyecto
- **Rutas/modelado:** Los clientes se identifican por `DNI` (índice único, seeder idempotente por DNI para evitar duplicados).
- **Seeder (`DbSeeder`)**: corre al arranque (`Program.cs` → `MigrateAsync()` + `SeedAsync`). Repara datos existentes (p. ej. `FechaNacimiento.Year <= 1900`), así que los fixes de datos se hacen ahí, no en migraciones.
- **Fotos de cliente**: columna `Cliente.FotoUrl`; subida en `ClienteController` (métodos privados `GuardarFoto`/`EliminarFoto`, limit ~5MB, extensiones JPG/PNG/WebP/GIF/BMP). Almacenadas en `wwwroot/uploads/fotos/` (carpeta ignorada por Git, no versionar).
- **Avatares**: partial `_ClienteAvatar.cshtml` — muestra la foto si existe, si no un icono genérico `person-fill` (NO iniciales). Tamaño vía `ViewBag.AvatarSize`.
- **Sidebar de recepción**: partial `_RecepcionSidebar.cshtml` usando el layout `.recepcion-layout` / `.recepcion-content` definido en `site.css`. Todas las pantallas del rol recepcionista (Check-in, Cobros, Padrón, Registrar) deben montarlo.
- **Navbar roles**: `_Layout.cshtml` muestra rutas según el rol (Admin / Recepcionista / Instructor / Cliente).
- **Estados usados en recepción**: "ACCESO AUTORIZADO", "ACCESO DENEGADO - CUOTA VENCIDA", "SOCIO NO REGISTRADO".
- **Permisos**: helpers en `BaseController` (`PuedeGestionarClientes`, `PuedeGestionarEmpleados`); `ClienteController` las invoca para controlar acceso por rol.

## Notas
- No agregar comentarios al código salvo que se pidan explícitamente.
- No crear documentación/README proactivamente a menos que se solicite.
- El commit y push a GitHub solo cuando el usuario lo pida expresamente.