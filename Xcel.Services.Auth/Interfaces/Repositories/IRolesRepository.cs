﻿using Domain.Interfaces.Repositories.Shared;
using Xcel.Services.Auth.Models;

namespace Xcel.Services.Auth.Interfaces.Repositories;

public interface IRolesRepository : IGenericRepository<RoleEntity>
{
    Task<RoleEntity?> GetByNameInsensitiveAsync(string roleName, CancellationToken cancellationToken = default);
}