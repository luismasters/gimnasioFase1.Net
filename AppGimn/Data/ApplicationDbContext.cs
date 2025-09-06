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

        // ============ TABLAS DE NEGOCIO ============
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }

        // ============ MÉTODOS AUXILIARES ÚTILES ============
        // Estos sí son muy prácticos para tu día a día

        /// <summary>
        /// Obtiene clientes activos ordenados por apellido
        /// </summary>
        public IQueryable<Cliente> ClientesActivos =>
            Clientes.Where(c => c.EstaActivo)
                   .OrderBy(c => c.Apellido)
                   .ThenBy(c => c.Nombre);

        /// <summary>
        /// Busca clientes por cualquier campo
        /// </summary>
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

        /// <summary>
        /// Obtiene empleados activos ordenados por apellido
        /// </summary>
        public IQueryable<Empleado> EmpleadosActivos =>
            Empleados.Where(e => e.EstaActivo)
                    .OrderBy(e => e.Apellido)
                    .ThenBy(e => e.Nombre);

        /// <summary>
        /// Busca empleados por cualquier campo
        /// </summary>
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
    }
}