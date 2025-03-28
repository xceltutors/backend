﻿using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Infra.Repositories.Shared;

namespace Infra.Repositories;

internal class PersonsRepository(AppDbContext dbContext) : GenericRepository<Person>(dbContext), IPersonsRepository
{
    public async Task<Person?> FindByEmailAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        return await DbContext.Persons.FirstOrDefaultAsync(p => p.EmailAddress == emailAddress, cancellationToken);
    }
}
