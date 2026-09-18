using System.Linq.Expressions;
using AutoTrade.Core.Entities;
using AutoTrade.Core.Interfaces;

namespace AutoTrade.Infrastructure.Services;

public class Service<T> : IService<T> where T : BaseEntity
{
    private readonly IGenericRepository<T> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public Service(IGenericRepository<T> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _repository.FindAsync(predicate);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _repository.AddAsync(entity);
        await _unitOfWork.CommitAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _repository.Update(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        _repository.Delete(entity); // Repository'deki Delete metodunu çağırıyoruz
        await _unitOfWork.CommitAsync();
    }
}