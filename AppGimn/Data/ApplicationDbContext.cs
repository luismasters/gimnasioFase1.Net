using AppGimn.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppGimn.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============ RESTRICCIONES DE UNICIDAD ============
            // Evita clientes y empleados duplicados por DNI a nivel de base de datos.
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.DNI)
                .IsUnique()
                .HasFilter("[DNI] IS NOT NULL AND [DNI] <> ''")
                .HasDatabaseName("IX_Clientes_DNI");

            modelBuilder.Entity<Empleado>()
                .HasIndex(e => e.DNI)
                .IsUnique()
                .HasFilter("[DNI] IS NOT NULL AND [DNI] <> ''")
                .HasDatabaseName("IX_Empleados_DNI");
        }

        // ============ TABLAS DE NEGOCIO ============
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }
        public DbSet<EjercicioRutina> EjerciciosRutina { get; set; }
        public DbSet<EvaluacionFisica> EvaluacionesFisicas { get; set; }

        // ============ MÉTODOS AUXILIARES ÚTILES ============

        public IQueryable<Cliente> ClientesActivos =>
            Clientes.Where(c => c.EstaActivo)
                   .OrderBy(c => c.Apellido)
                   .ThenBy(c => c.Nombre);

        public IQueryable<Cliente> BuscarClientes(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return ClientesActivos;

            termino = termino.ToLower().Trim();

            return Clientes.Where(c =>
                c.Nombre.ToLower().Contains(termino) ||
                c.Apellido.ToLower().Contains(termino) ||
                c.DNI.Contains(termino) ||
                (c.Email != null && c.Email.ToLower().Contains(termino)))
                .OrderBy(c => c.Apellido);
        }

        public IQueryable<Empleado> EmpleadosActivos =>
            Empleados.Where(e => e.EstaActivo)
                    .OrderBy(e => e.Apellido)
                    .ThenBy(e => e.Nombre);

        public IQueryable<Empleado> BuscarEmpleados(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return EmpleadosActivos;

            termino = termino.ToLower().Trim();

            return Empleados.Where(e =>
                e.Nombre.ToLower().Contains(termino) ||
                e.Apellido.ToLower().Contains(termino) ||
                e.DNI.Contains(termino) ||
                e.Cargo.ToLower().Contains(termino))
                .OrderBy(e => e.Apellido);
        }

        public async Task<Empleado?> ObtenerEmpleadoPorUsuario(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.DNI))
                return null;

            return await Empleados
                .FirstOrDefaultAsync(e => e.DNI == usuario.DNI && e.EstaActivo);
        }
    }
}