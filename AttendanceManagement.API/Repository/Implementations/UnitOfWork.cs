
using AttendanceManagement.API.Data;
using AttendanceManagement.API.Repository.Implementations;
using AttendanceManagement.API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;
    private bool _disposed = false;


    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    private IAdminRepository? _adminRepository;
    private IWorkerRepository? _workerRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : class
    {

        if (_repositories.TryGetValue(typeof(T), out var repo))
        {
            return (IRepository<T>)repo;
        }

        var newRepo = new Repository<T>(_context);
        _repositories[typeof(T)] = newRepo;
        return newRepo;
    }

    public IAdminRepository Admins
    {
        get
        {
            if (_adminRepository == null)
            {
                _adminRepository = new AdminRepository(this);
            }
            return _adminRepository;
        }
    }

    public IWorkerRepository Workers
    {
        get
        {
            if (_workerRepository == null)
            {
                _workerRepository = new WorkerRepository(this);
            }
            return _workerRepository;
        }
    }

    public IAdminRepository AdminRepository => throw new NotImplementedException();

    public IWorkerRepository WorkerRepository => throw new NotImplementedException();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
