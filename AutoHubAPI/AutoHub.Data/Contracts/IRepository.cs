﻿namespace AutoHub.Data.Contracts;

public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    Task<TEntity> Get(Guid id);
    Task<ICollection<TEntity>> GetAll();
}