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

            // 3. Sembrar Membresías
            if (!await context.Membresias.AnyAsync())
            {
                var membresias = new List<Membresia>
                {
                    new Membresia
                    {
                        Nombre = "Pase Premium Aura 24/7",
                        Precio = 75000,
                        DuracionDias = 30,
                        Descripcion = "Acceso libre 24/7 a musculación, cardio y clases grupales ilimitadas.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase Estándar Mensual",
                        Precio = 45000,
                        DuracionDias = 30,
                        Descripcion = "Acceso a sala de musculación en horario regular.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase VIP Corporativo",
                        Precio = 120000,
                        DuracionDias = 30,
                        Descripcion = "Entrenador personal dedicado, locker privado y acceso al Spa.",
                        EstaActivo = true
                    }
                };

                await context.Membresias.AddRangeAsync(membresias);
                await context.SaveChangesAsync();
            }

            // 4. Sembrar Pagos Iniciales
            if (!await context.Pagos.AnyAsync())
            {
                var clienteCarlos = await context.Clientes.FirstOrDefaultAsync(c => c.DNI == "11223344");
                var membresiaPremium = await context.Membresias.FirstOrDefaultAsync(m => m.Nombre.Contains("Premium"));

                if (clienteCarlos != null && membresiaPremium != null)
                {
                    var pagos = new List<Pago>
                    {
                        new Pago
                        {
                            ClienteId = clienteCarlos.Id,
                            MembresiaId = membresiaPremium.Id,
                            Monto = 75000,
                            FechaPago = DateTime.Now.AddDays(-5),
                            FechaVencimiento = DateTime.Now.AddDays(25),
                            MedioPago = "Efectivo",
                            ComprobanteNumero = "REC-00984",
                            RecepcionistaEmail = "recepcion@gimnasio.com"
                        }
                    };

                    await context.Pagos.AddRangeAsync(pagos);
                    await context.SaveChangesAsync();
                }
            }

            // 5. Sembrar Evaluaciones Físicas
            if (!await context.EvaluacionesFisicas.AnyAsync())
            {
                var clienteCarlos = await context.Clientes.FirstOrDefaultAsync(c => c.DNI == "11223344");
                if (clienteCarlos != null)
                {
                    var evaluaciones = new List<EvaluacionFisica>
                    {
                        new EvaluacionFisica
                        {
                            ClienteId = clienteCarlos.Id,
                            FechaEvaluacion = DateTime.Now.AddMonths(-2),
                            PesoKg = 82.5,
                            PorcentajeGrasa = 18.2,
                            MasaMuscularKg = 39.7,
                            ToraxCm = 100,
                            CinturaCm = 87,
                            BicepsCm = 37.0,
                            Observaciones = "Evaluación inicial de ingreso"
                        },
                        new EvaluacionFisica
                        {
                            ClienteId = clienteCarlos.Id,
                            FechaEvaluacion = DateTime.Now.AddMonths(-1),
                            PesoKg = 80.2,
                            PorcentajeGrasa = 16.8,
                            MasaMuscularKg = 40.5,
                            ToraxCm = 102,
                            CinturaCm = 84,
                            BicepsCm = 37.8,
                            Observaciones = "Re-evaluación mes 2"
                        },
                        new EvaluacionFisica
                        {
                            ClienteId = clienteCarlos.Id,
                            FechaEvaluacion = DateTime.Now.AddDays(-5),
                            PesoKg = 78.5,
                            PorcentajeGrasa = 14.2,
                            MasaMuscularKg = 41.8,
                            ToraxCm = 104,
                            CinturaCm = 81,
                            BicepsCm = 38.5,
                            Observaciones = "Excelente avance en hipertrofia y reducción de grasa"
                        }
                    };

                    await context.EvaluacionesFisicas.AddRangeAsync(evaluaciones);
                    await context.SaveChangesAsync();
                }
            }

            // 6. Sembrar Usuarios de Identity
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
