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

            // ================= 1. SEMBRAR EMPLEADOS =================
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
                        FechaIngreso = DateTime.Now.AddYears(-3),
                        Salario = 450000,
                        EstaActivo = true,
                        Observaciones = "Gerencia General & Administración Central"
                    },
                    new Empleado
                    {
                        Nombre = "Ana",
                        Apellido = "Martínez",
                        DNI = "44556677",
                        Cargo = "Recepcionista",
                        Telefono = "1155667788",
                        Email = "recepcion@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddMonths(-8),
                        Salario = 220000,
                        EstaActivo = true,
                        Observaciones = "Turno Mañana (08:00 a 16:00 hs)"
                    },
                    new Empleado
                    {
                        Nombre = "Diego",
                        Apellido = "Silva",
                        DNI = "44998877",
                        Cargo = "Recepcionista",
                        Telefono = "1199881122",
                        Email = "diego.recepcion@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddMonths(-4),
                        Salario = 220000,
                        EstaActivo = true,
                        Observaciones = "Turno Tarde/Noche (16:00 a 23:00 hs)"
                    },
                    new Empleado
                    {
                        Nombre = "Lucas",
                        Apellido = "Rodríguez",
                        DNI = "55667788",
                        Cargo = "Entrenador",
                        Telefono = "1166778899",
                        Email = "instructor@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddYears(-2),
                        Salario = 280000,
                        EstaActivo = true,
                        Observaciones = "Head Coach - Musculación & Biomecánica"
                    },
                    new Empleado
                    {
                        Nombre = "Martina",
                        Apellido = "Herrera",
                        DNI = "55112233",
                        Cargo = "Entrenadora",
                        Telefono = "1122113344",
                        Email = "martina.herrera@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddYears(-1),
                        Salario = 260000,
                        EstaActivo = true,
                        Observaciones = "Instructora de Pilates Reformer & Funcional"
                    },
                    new Empleado
                    {
                        Nombre = "Gonzalo",
                        Apellido = "Acosta",
                        DNI = "55334455",
                        Cargo = "Entrenador",
                        Telefono = "1133445566",
                        Email = "gonzalo.acosta@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddMonths(-9),
                        Salario = 270000,
                        EstaActivo = true,
                        Observaciones = "Coach de HIIT & CrossFit"
                    }
                };

                await context.Empleados.AddRangeAsync(empleados);
                await context.SaveChangesAsync();
            }

            // ================= 2. SEMBRAR CLIENTES =================
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
                        FechaInscripcion = DateTime.Now.AddMonths(-6),
                        ContactoEmergencia = "Padre: Roberto Gómez - 1144001122",
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
                        FechaInscripcion = DateTime.Now.AddMonths(-3),
                        ContactoEmergencia = "Hermano: Fernando López - 1199002233",
                        ObservacionesMedicas = "Sin observaciones médicas.",
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
                        FechaInscripcion = DateTime.Now.AddMonths(-5),
                        ContactoEmergencia = "Esposa: Laura Pérez - 1122003344",
                        ObservacionesMedicas = "Hipertensión leve controlada.",
                        EstaActivo = true
                    },
                    new Cliente
                    {
                        Nombre = "Sofía",
                        Apellido = "Ramírez",
                        DNI = "44112233",
                        FechaNacimiento = new DateTime(1998, 3, 25),
                        Telefono = "1133221100",
                        Email = "sofia.ramirez@gmail.com",
                        FechaInscripcion = DateTime.Now.AddMonths(-4),
                        ContactoEmergencia = "Madre: Elena Ramírez - 1133009988",
                        ObservacionesMedicas = "Apto físico de alto nivel.",
                        EstaActivo = true
                    },
                    new Cliente
                    {
                        Nombre = "Lucas",
                        Apellido = "Fernández",
                        DNI = "55223344",
                        FechaNacimiento = new DateTime(1992, 11, 4),
                        Telefono = "1155443322",
                        Email = "lucas.f@gmail.com",
                        FechaInscripcion = DateTime.Now.AddMonths(-2),
                        ContactoEmergencia = "Amigo: Esteban - 1155006677",
                        ObservacionesMedicas = "Recuperación de rodilla izquierda.",
                        EstaActivo = true
                    },
                    new Cliente
                    {
                        Nombre = "Valentina",
                        Apellido = "Rossi",
                        DNI = "66334455",
                        FechaNacimiento = new DateTime(1994, 7, 18),
                        Telefono = "1166554433",
                        Email = "valen.rossi@gmail.com",
                        FechaInscripcion = DateTime.Now.AddMonths(-7),
                        ContactoEmergencia = "Pareja: Tomás Rossi - 1166008899",
                        ObservacionesMedicas = "Apto médico al día.",
                        EstaActivo = true
                    },
                    new Cliente
                    {
                        Nombre = "Mateo",
                        Apellido = "Benítez",
                        DNI = "77445566",
                        FechaNacimiento = new DateTime(1991, 1, 30),
                        Telefono = "1177665544",
                        Email = "mateo.b@gmail.com",
                        FechaInscripcion = DateTime.Now.AddMonths(-1),
                        ContactoEmergencia = "Hermana: Luciana Benítez - 1177001122",
                        ObservacionesMedicas = "Sin afecciones de salud.",
                        EstaActivo = true
                    },
                    new Cliente
                    {
                        Nombre = "Camila",
                        Apellido = "Torres",
                        DNI = "88556677",
                        FechaNacimiento = new DateTime(1997, 9, 14),
                        Telefono = "1188776655",
                        Email = "cami.torres@gmail.com",
                        FechaInscripcion = DateTime.Now.AddDays(-20),
                        ContactoEmergencia = "Madre: Andrea Torres - 1188003344",
                        ObservacionesMedicas = "Pase VIP corporativo.",
                        EstaActivo = true
                    },
                    new Cliente
                    {
                        Nombre = "Nicolás",
                        Apellido = "Morales",
                        DNI = "99667788",
                        FechaNacimiento = new DateTime(1989, 4, 8),
                        Telefono = "1199001122",
                        Email = "nico.morales@gmail.com",
                        FechaInscripcion = DateTime.Now.AddMonths(-8),
                        ContactoEmergencia = "Esposa: Patricia - 1199112233",
                        ObservacionesMedicas = "Asiste en turno mañana.",
                        EstaActivo = true
                    },
                    new Cliente
                    {
                        Nombre = "Lucía",
                        Apellido = "Castro",
                        DNI = "12345678",
                        FechaNacimiento = new DateTime(1993, 6, 22),
                        Telefono = "1123456789",
                        Email = "lucia.castro@gmail.com",
                        FechaInscripcion = DateTime.Now.AddMonths(-5),
                        ContactoEmergencia = "Padre: Miguel Castro - 1123004455",
                        ObservacionesMedicas = "Apto para Pilates y Musculación.",
                        EstaActivo = true
                    }
                };

                await context.Clientes.AddRangeAsync(clientes);
                await context.SaveChangesAsync();
            }

            // ================= 3. SEMBRAR MEMBRESÍAS / PLANES =================
            if (!await context.Membresias.AnyAsync())
            {
                var membresias = new List<Membresia>
                {
                    new Membresia
                    {
                        Nombre = "Pase Premium Aura 24/7",
                        Precio = 75000,
                        DuracionDias = 30,
                        Descripcion = "Acceso libre 24/7 a musculación, cardio y clases grupales ilimitadas con sauna y locker.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase Estándar Mensual",
                        Precio = 45000,
                        DuracionDias = 30,
                        Descripcion = "Acceso completo a la sala de musculación y área de cardio en horario regular.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase VIP Corporativo",
                        Precio = 120000,
                        DuracionDias = 30,
                        Descripcion = "Entrenador personal dedicado, evaluaciones bioimpedancia semanales y acceso al Spa.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase Estudiantil / Universitario",
                        Precio = 35000,
                        DuracionDias = 30,
                        Descripcion = "Descuento especial con certificado de alumno regular. Horario flexible.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase Anual Elite Aura",
                        Precio = 650000,
                        DuracionDias = 365,
                        Descripcion = "Membresía anual con 2 meses de regalo y kit oficial Aura Club.",
                        EstaActivo = true
                    }
                };

                await context.Membresias.AddRangeAsync(membresias);
                await context.SaveChangesAsync();
            }

            // ================= 4. SEMBRAR PAGOS HISTÓRICOS =================
            if (!await context.Pagos.AnyAsync())
            {
                var clientesList = await context.Clientes.ToListAsync();
                var premiumPlan = await context.Membresias.FirstOrDefaultAsync(m => m.Nombre.Contains("Premium"));
                var estandarPlan = await context.Membresias.FirstOrDefaultAsync(m => m.Nombre.Contains("Estándar"));
                var vipPlan = await context.Membresias.FirstOrDefaultAsync(m => m.Nombre.Contains("VIP"));

                var pagos = new List<Pago>();
                int recIndex = 980;

                foreach (var c in clientesList)
                {
                    var planSelected = c.DNI == "44112233" || c.DNI == "88556677" 
                        ? vipPlan 
                        : (c.Id % 2 == 0 ? premiumPlan : estandarPlan);

                    bool esVencido = c.DNI == "22334455"; // María López vencida para prueba

                    pagos.Add(new Pago
                    {
                        ClienteId = c.Id,
                        MembresiaId = planSelected?.Id,
                        Monto = planSelected?.Precio ?? 45000,
                        FechaPago = esVencido ? DateTime.Now.AddDays(-35) : DateTime.Now.AddDays(-Random.Shared.Next(2, 20)),
                        FechaVencimiento = esVencido ? DateTime.Now.AddDays(-5) : DateTime.Now.AddDays(Random.Shared.Next(10, 28)),
                        MedioPago = c.Id % 4 == 0 ? "Efectivo" : (c.Id % 4 == 1 ? "MercadoPago" : (c.Id % 4 == 2 ? "Debito" : "Credito")),
                        ComprobanteNumero = $"REC-{recIndex++}",
                        RecepcionistaEmail = "recepcion@gimnasio.com"
                    });
                }

                await context.Pagos.AddRangeAsync(pagos);
                await context.SaveChangesAsync();
            }

            // ================= 5. SEMBRAR EVALUACIONES FÍSICAS =================
            if (!await context.EvaluacionesFisicas.AnyAsync())
            {
                var carlos = await context.Clientes.FirstOrDefaultAsync(c => c.DNI == "11223344");
                var sofia = await context.Clientes.FirstOrDefaultAsync(c => c.DNI == "44112233");

                var evals = new List<EvaluacionFisica>();

                if (carlos != null)
                {
                    evals.Add(new EvaluacionFisica
                    {
                        ClienteId = carlos.Id,
                        FechaEvaluacion = DateTime.Now.AddMonths(-3),
                        PesoKg = 84.0,
                        PorcentajeGrasa = 19.5,
                        MasaMuscularKg = 38.5,
                        ToraxCm = 98,
                        CinturaCm = 89,
                        BicepsCm = 36.0,
                        Observaciones = "Evaluación diagnóstica inicial."
                    });
                    evals.Add(new EvaluacionFisica
                    {
                        ClienteId = carlos.Id,
                        FechaEvaluacion = DateTime.Now.AddMonths(-1),
                        PesoKg = 80.5,
                        PorcentajeGrasa = 16.0,
                        MasaMuscularKg = 40.2,
                        ToraxCm = 102,
                        CinturaCm = 83,
                        BicepsCm = 37.5,
                        Observaciones = "Gran progreso en hipertrofia y reducción de grasa abdominal."
                    });
                    evals.Add(new EvaluacionFisica
                    {
                        ClienteId = carlos.Id,
                        FechaEvaluacion = DateTime.Now.AddDays(-4),
                        PesoKg = 78.5,
                        PorcentajeGrasa = 14.2,
                        MasaMuscularKg = 41.8,
                        ToraxCm = 104,
                        CinturaCm = 81,
                        BicepsCm = 38.5,
                        Observaciones = "Excelente estado físico y masa muscular magra destacada."
                    });
                }

                if (sofia != null)
                {
                    evals.Add(new EvaluacionFisica
                    {
                        ClienteId = sofia.Id,
                        FechaEvaluacion = DateTime.Now.AddMonths(-2),
                        PesoKg = 62.0,
                        PorcentajeGrasa = 22.0,
                        MasaMuscularKg = 26.5,
                        ToraxCm = 88,
                        CinturaCm = 68,
                        BicepsCm = 27.0,
                        Observaciones = "Objetivo: Tonificación y resistencia."
                    });
                    evals.Add(new EvaluacionFisica
                    {
                        ClienteId = sofia.Id,
                        FechaEvaluacion = DateTime.Now.AddDays(-10),
                        PesoKg = 59.5,
                        PorcentajeGrasa = 18.8,
                        MasaMuscularKg = 28.1,
                        ToraxCm = 90,
                        CinturaCm = 65,
                        BicepsCm = 28.2,
                        Observaciones = "Reducción de grasa visceral y aumento de fuerza en tren inferior."
                    });
                }

                await context.EvaluacionesFisicas.AddRangeAsync(evals);
                await context.SaveChangesAsync();
            }

            // ================= 6. SEMBRAR RUTINAS & EJERCICIOS =================
            if (!await context.Rutinas.AnyAsync())
            {
                var carlos = await context.Clientes.FirstOrDefaultAsync(c => c.DNI == "11223344");
                var coach = await context.Empleados.FirstOrDefaultAsync(e => e.Cargo == "Entrenador");

                if (carlos != null)
                {
                    var rutinaCarlos = new Rutina
                    {
                        ClienteId = carlos.Id,
                        InstructorId = coach?.Id,
                        Nombre = "Hipertrofia & Fuerza - Nivel Avanzado",
                        FechaCreacion = DateTime.Now.AddDays(-15),
                        DiaSemana = "Lunes: Pecho, Hombros & Tríceps",
                        EstaActiva = true
                    };

                    await context.Rutinas.AddAsync(rutinaCarlos);
                    await context.SaveChangesAsync();

                    var ejercicios = new List<EjercicioRutina>
                    {
                        new EjercicioRutina
                        {
                            RutinaId = rutinaCarlos.Id,
                            NombreEjercicio = "Press de Banca Inclinado con Barra",
                            Series = 4,
                            Repeticiones = "10 - 12",
                            CargaSugerida = "75 kg",
                            DescansoSegundos = 90,
                            ImagenUrl = "/images/landing/weight_training.jpg"
                        },
                        new EjercicioRutina
                        {
                            RutinaId = rutinaCarlos.Id,
                            NombreEjercicio = "Aperturas en Banco Plano con Mancuernas",
                            Series = 4,
                            Repeticiones = "12 - 15",
                            CargaSugerida = "22 kg c/u",
                            DescansoSegundos = 60,
                            ImagenUrl = "/images/landing/aura_strength.jpg"
                        },
                        new EjercicioRutina
                        {
                            RutinaId = rutinaCarlos.Id,
                            NombreEjercicio = "Press Militar con Barra de Pie",
                            Series = 4,
                            Repeticiones = "8 - 10",
                            CargaSugerida = "50 kg",
                            DescansoSegundos = 90,
                            ImagenUrl = "/images/landing/aura_hero.jpg"
                        },
                        new EjercicioRutina
                        {
                            RutinaId = rutinaCarlos.Id,
                            NombreEjercicio = "Fondos en Paralelas para Tríceps con Lastre",
                            Series = 3,
                            Repeticiones = "12",
                            CargaSugerida = "+15 kg",
                            DescansoSegundos = 60,
                            ImagenUrl = "/images/landing/group_classes.jpg"
                        }
                    };

                    await context.EjerciciosRutina.AddRangeAsync(ejercicios);
                    await context.SaveChangesAsync();
                }
            }

            // ================= 7. SEMBRAR ASISTENCIAS REALES =================
            if (!await context.Asistencias.AnyAsync())
            {
                var clientesList = await context.Clientes.Take(6).ToListAsync();
                var asistencias = new List<Asistencia>();

                foreach (var c in clientesList)
                {
                    bool alDia = c.DNI != "22334455";

                    asistencias.Add(new Asistencia
                    {
                        ClienteId = c.Id,
                        FechaHoraIngreso = DateTime.Now.AddHours(-Random.Shared.Next(1, 4)),
                        Permitido = alDia,
                        MotivoDenegado = alDia ? null : "Cuota vencida el 01/08"
                    });
                }

                await context.Asistencias.AddRangeAsync(asistencias);
                await context.SaveChangesAsync();
            }

            // ================= 8. SEMBRAR USUARIOS DE IDENTITY =================
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
