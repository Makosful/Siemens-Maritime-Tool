﻿using System.Collections.Generic;

namespace Schwartz.Siemens.Core.DomainServices.Repositories
{
    public interface ICrudRepositoryRead<out T>
    {
        T Read(int id);

        IEnumerable<T> ReadAll();
    }

    public interface ICrudRepositoryWrite<T>
    {
        T Create(T item);

        T Update(int id, T item);

        T Delete(T item);
    }

    public interface ICrudRepository<T> : ICrudRepositoryRead<T>, ICrudRepositoryWrite<T>
    {
    }
}