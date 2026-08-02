using AppGimn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppGimn.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

            // Asegurar que la base de datos esté creada y al día con las migraciones
            await context.Database.MigrateAsync();

            // 1. Sembrar Empleados
            if (!await context.Empleados.AnyAsync())
            {
                var empleados = new List<Empleado>
                {
                    new Empleado
                    {
                        Nombre = "Administrador",
                        Apellido = "Sistema",
                        DNI = "00000000",
                        Cargo = "Gerente",
                        Telefono = "1100000000",
                        Email = "admin@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddYears(-2),
                        Salario = 350000,
                        EstaActivo = true,
                        Observaciones = "Usuario Gerente Principal"
                    },
                    new Empleado
                    {
                        Nombre = "Ana",
                        Apellido = "Martínez",
                        DNI = "44556677",
                        Cargo = "Recepcionista",
                        Telefono = "1155667788",
                        Email = "recepcion@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddMonths(-6),
                        Salario = 180000,
                        EstaActivo = true,
                        Observaciones = "Turno Mañana - Atención al Cliente"
                    },
                    new Empleado
                    {
                        Nombre = "Lucas",
                        Apellido = "Rodríguez",
                        DNI = "55667788",
                        Cargo = "Entrenador",
                        Telefono = "1166778899",
                        Email = "instructor@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddYears(-1),
                        Salario = 220000,
                        EstaActivo = true,
                        Observaciones = "Head Coach & Especialista en Musculación"
                    }
                };

                await context.Empleados.AddRangeAsync(empleados);
                await context.SaveChangesAsync();
            }

            // 2. Sembrar Clientes
            if (!await context.Clientes.AnyAsync())
            {
                var clientes = new List<Cliente>
                {
                    new Cliente
                    {
                        Nombre = "Carlos",
                        Apellido = "Gómez",
                        DNI = "11223344",
                        FechaNacimiento = new DateTime(1990, 5, 15),
                        Telefono = "1144556677",
                        Email = "cliente@gimnasio.com",
                        FechaInscripcion = DateTime.Now.AddMonths(-3),
                        ContactoEmergencia = "Padre - 1144001122",
                        ObservacionesMedicas = "Apto médico presentado. Apto para alto rendimiento.",
                        EstaActivo = true
                    },
                    new Cliente
                    {
                        Nombre = "María",
                        Apellido = "López",
                        DNI = "22334455",
                        FechaNacimiento = new DateTime(1995, 8, 20),
                        Telefono = "1199887766",
                        Email = "maria.lopez@gmail.com",
                        FechaInscripcion = DateTime.Now.AddMonths(-1),
                        ContactoEmergencia = "Hermano - 1199002233",
                        ObservacionesMedicas = "Sin observaciones.",
                        EstaActivo = true
                    },
                    new Cliente
                    {
                        Nombre = "Juan",
                        Apellido = "Pérez",
                        DNI = "33445566",
                        FechaNacimiento = new DateTime(1988, 12, 10),
                        Telefono = "1122334455",
                        Email = "juan.perez@gmail.com",
                        FechaInscripcion = DateTime.Now.AddDays(-10),
                        ContactoEmergencia = "Esposa - 1122003344",
                        ObservacionesMedicas = "Hipertensión leve controlada.",
                        EstaActivo = true
                    }
                };

                await context.Clientes.AddRangeAsync(clientes);
                await context.SaveChangesAsync();
            }

            // 3. Sembrar Usuarios de Identity para cada uno de los 4 Roles Demo
            await CrearUsuarioSiNoExiste(userManager, "admin@gimnasio.com", "Admin123!", "00000000", esAdmin: true, esEmpleado: true, esCliente: false);
            await CrearUsuarioSiNoExiste(userManager, "recepcion@gimnasio.com", "Recep123!", "44556677", esAdmin: false, esEmpleado: true, esCliente: false);
            await CrearUsuarioSiNoExiste(userManager, "instructor@gimnasio.com", "Coach123!", "55667788", esAdmin: false, esEmpleado: true, esCliente: false);
            await CrearUsuarioSiNoExiste(userManager, "cliente@gimnasio.com", "Cliente123!", "11223344", esAdmin: false, esEmpleado: false, esCliente: true);
        }

        private static async Task CrearUsuarioSiNoExiste(UserManager<Usuario> userManager, string email, string password, string dni, bool esAdmin, bool esEmpleado, bool esCliente)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new Usuario
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DNI = dni,
                    EsAdmin = esAdmin,
                    EsEmpleado = esEmpleado,
                    EsCliente = esCliente
                };
                await userManager.CreateAsync(user, password);
            }
        }
    }
}
