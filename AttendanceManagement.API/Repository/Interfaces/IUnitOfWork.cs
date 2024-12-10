
using AttendanceManagement.API.Repository.Interfaces;


public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : class;
    IAdminRepository AdminRepository { get; }
    IWorkerRepository WorkerRepository { get; }
    Task<int> SaveChangesAsync();
    void Dispose();
}
