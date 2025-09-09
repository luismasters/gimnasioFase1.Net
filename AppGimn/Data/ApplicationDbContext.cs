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

        // ============ MÉTODOS PARA VINCULAR USUARIO-EMPLEADO ============
        // Agregar estos métodos a tu ApplicationDbContext

        /// <summary>
        /// Busca el empleado vinculado a un usuario por DNI
        /// </summary>
        public async Task<Empleado?> ObtenerEmpleadoPorUsuario(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.DNI))
                return null;

            return await Empleados
                .FirstOrDefaultAsync(e => e.DNI == usuario.DNI && e.EstaActivo);
        }

        /// <summary>
        /// Verifica si un usuario tiene un empleado vinculado
        /// </summary>
        public async Task<bool> UsuarioTieneEmpleado(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.DNI))
                return false;

            return await Empleados
                .AnyAsync(e => e.DNI == usuario.DNI && e.EstaActivo);
        }

        /// <summary>
        /// Obtiene todos los empleados que NO tienen usuario vinculado
        /// </summary>
        public IQueryable<Empleado> EmpleadosSinUsuario =>
            Empleados.Where(e => e.EstaActivo && !Users.Any(u => u.DNI == e.DNI));

        /// <summary>
        /// Obtiene todos los usuarios que NO tienen empleado vinculado
        /// </summary>
        public IQueryable<Usuario> UsuariosSinEmpleado =>
            Users.Where(u => !string.IsNullOrEmpty(u.DNI) &&
                             u.EsEmpleado &&
                             !Empleados.Any(e => e.DNI == u.DNI && e.EstaActivo));





    }
}