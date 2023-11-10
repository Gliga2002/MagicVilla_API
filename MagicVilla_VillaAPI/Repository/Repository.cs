using System.Linq.Expressions;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Repository
{
  public class Repository<T> : IRepository<T> where T : class
  {
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;
    public Repository(ApplicationDbContext db)
    {
      _db = db;
      // _db.VillaNumbers.Include(u => u.Villa).ToList();

      // create new DbSet based on Type <T> you have
      // For example you can pass Villas
      this.dbSet = _db.Set<T>();
    }
    public async Task CreateAsync(T entity)
    {
      await dbSet.AddAsync(entity);
      await SaveAsync();
    }

  // "Villa,VillaSpecial"
    public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, string? includeProperties = null)
    {
      //IQueryable on Villa
      IQueryable<T> query = dbSet;
      // nece odma da se izvrsi mozes da ga modifikujes
      if (!tracked)
      {
        query = query.AsNoTracking();
      }
      if (filter != null)
      {
        query = query.Where(filter);
      }
      if ( includeProperties != null)
      {
        foreach(var includeProp in includeProperties.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
        {
          query = query.Include(includeProp);
        }
      }
      // ovde ce da se izvrsi jer ToList odma izvrsava
      return await query.FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
      //IQueryable on Villa
      IQueryable<T> query = dbSet;
      // nece odma da se izvrsi mozes da ga modifikujes
      if (filter != null)
      {
        query = query.Where(filter);
      }
      if (includeProperties != null)
      {
        foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProp);
        }
      }
      // ovde ce da se izvrsi jer ToList odma izvrsava
      return await query.ToListAsync();
    }

    public async Task RemoveAsync(T entity)
    {
      dbSet.Remove(entity);
      await SaveAsync();
    }

    public async Task SaveAsync()
    {
      await _db.SaveChangesAsync();
    }

  }
}