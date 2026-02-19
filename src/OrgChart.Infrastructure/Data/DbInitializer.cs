using Microsoft.EntityFrameworkCore;
using OrgChart.Domain.Entities;
using OrgChart.Domain.Enums;
using OrgChart.Domain.ValueObjects;

namespace OrgChart.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(OrgChartDbContext context)
    {
        // Aplica migrations pendentes
        await context.Database.MigrateAsync();

        // Se já existem dados, não faz seed
        if (await context.Departments.AnyAsync())
            return;

        // Seed Departments
        var departments = new List<Department>
        {
            new Department("Tecnologia", "TI", true),
            new Department("Recursos Humanos", "RH", true),
            new Department("Financeiro", "FIN", true),
            new Department("Comercial", "COM", true),
            new Department("Marketing", "MKT", true)
        };

        await context.Departments.AddRangeAsync(departments);
        await context.SaveChangesAsync();

        // Seed Positions
        var positions = new List<Position>
        {
            new Position("CEO", EPositionLevel.Director, true),
            new Position("Diretor de TI", EPositionLevel.Director, true),
            new Position("Diretor de RH", EPositionLevel.Director, true),
            new Position("Gerente de Desenvolvimento", EPositionLevel.Manager, true),
            new Position("Gerente de Infraestrutura", EPositionLevel.Manager, true),
            new Position("Coordenador de Projetos", EPositionLevel.Coordinator, true),
            new Position("Desenvolvedor Sênior", EPositionLevel.Senior, true),
            new Position("Desenvolvedor Pleno", EPositionLevel.MidLevel, true),
            new Position("Desenvolvedor Júnior", EPositionLevel.Junior, true),
            new Position("Estagiário de Desenvolvimento", EPositionLevel.Intern, true),
            new Position("Analista de RH Sênior", EPositionLevel.Senior, true),
            new Position("Analista de RH Pleno", EPositionLevel.MidLevel, true)
        };

        await context.Positions.AddRangeAsync(positions);
        await context.SaveChangesAsync();

        // Seed Employees
        var ceo = new Employee(
            "João Silva",
            Email.Create("joao.silva@orgchart.com"),
            departments[0].Id, // TI
            positions[0].Id,   // CEO
            new DateTime(2020, 1, 1),
            null
        );
        await context.Employees.AddAsync(ceo);
        await context.SaveChangesAsync();

        var directorTI = new Employee(
            "Maria Santos",
            Email.Create("maria.santos@orgchart.com"),
            departments[0].Id,
            positions[1].Id,
            new DateTime(2020, 3, 15),
            ceo.Id
        );
        await context.Employees.AddAsync(directorTI);

        var directorRH = new Employee(
            "Carlos Oliveira",
            Email.Create("carlos.oliveira@orgchart.com"),
            departments[1].Id,
            positions[2].Id,
            new DateTime(2020, 3, 20),
            ceo.Id
        );
        await context.Employees.AddAsync(directorRH);
        await context.SaveChangesAsync();

        var managerDev = new Employee(
            "Ana Costa",
            Email.Create("ana.costa@orgchart.com"),
            departments[0].Id,
            positions[3].Id,
            new DateTime(2020, 6, 1),
            directorTI.Id
        );
        await context.Employees.AddAsync(managerDev);

        var managerInfra = new Employee(
            "Pedro Almeida",
            Email.Create("pedro.almeida@orgchart.com"),
            departments[0].Id,
            positions[4].Id,
            new DateTime(2020, 6, 15),
            directorTI.Id
        );
        await context.Employees.AddAsync(managerInfra);
        await context.SaveChangesAsync();

        var coordinator = new Employee(
            "Julia Ferreira",
            Email.Create("julia.ferreira@orgchart.com"),
            departments[0].Id,
            positions[5].Id,
            new DateTime(2021, 1, 10),
            managerDev.Id
        );
        await context.Employees.AddAsync(coordinator);
        await context.SaveChangesAsync();

        var devs = new List<Employee>
        {
            new Employee("Lucas Rodrigues", Email.Create("lucas.rodrigues@orgchart.com"),
                departments[0].Id, positions[6].Id, new DateTime(2021, 2, 1), coordinator.Id),
            new Employee("Fernanda Lima", Email.Create("fernanda.lima@orgchart.com"),
                departments[0].Id, positions[7].Id, new DateTime(2021, 5, 15), coordinator.Id),
            new Employee("Rafael Souza", Email.Create("rafael.souza@orgchart.com"),
                departments[0].Id, positions[8].Id, new DateTime(2022, 1, 10), coordinator.Id),
            new Employee("Camila Martins", Email.Create("camila.martins@orgchart.com"),
                departments[0].Id, positions[9].Id, new DateTime(2023, 7, 1), coordinator.Id),

            new Employee("Beatriz Gomes", Email.Create("beatriz.gomes@orgchart.com"),
                departments[1].Id, positions[10].Id, new DateTime(2021, 3, 1), directorRH.Id),
            new Employee("Roberto Dias", Email.Create("roberto.dias@orgchart.com"),
                departments[1].Id, positions[11].Id, new DateTime(2022, 8, 15), directorRH.Id)
        };

        await context.Employees.AddRangeAsync(devs);
        await context.SaveChangesAsync();
    }
}
