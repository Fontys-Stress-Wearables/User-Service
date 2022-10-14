﻿namespace User_Service.Interfaces;

public interface IGenericRepository<T> where T : class
{
    T? GetById(string id);
    IEnumerable<T> GetAll();
    T Add(T entity);
    T Update(T entity);
    void Remove(T entity);
}
