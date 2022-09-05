using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Infrastructure.Services
{
   public class UserService : IUserService
   {
      private readonly InMemoryContext _dbContext;

      public UserService(InMemoryContext dbContext)
      {
         _dbContext = dbContext;

         // this is a hack to seed data into the in memory database. Do not use this in production.
         _dbContext.Database.EnsureCreated();
      }

      /// <inheritdoc />
      public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default)
      {
         User? user = await _dbContext.Users.Where(user => user.Id == id)
                                      .Include(x => x.ContactDetail)
                                      .FirstOrDefaultAsync(cancellationToken);

         return user;
      }

      /// <inheritdoc />
      public async Task<IEnumerable<User>> FindAsync(string? givenNames, string? lastName, CancellationToken cancellationToken = default)
      {
         return await _dbContext.Users.Where(user => string.Equals(user.GivenNames, givenNames, StringComparison.OrdinalIgnoreCase) || string.Equals(user.LastName, lastName, StringComparison.OrdinalIgnoreCase)).ToListAsync();
      }

      /// <inheritdoc />
      public async Task<IEnumerable<User>> GetPaginatedAsync(int page, int count, CancellationToken cancellationToken = default)
      {
         return await _dbContext.Users.Skip(((page - 1) * count) + 1).Take(count).ToListAsync();
      }

      /// <inheritdoc />
      public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
      {
         await _dbContext.Users.AddAsync(user);
         await _dbContext.SaveChangesAsync();
         return user;
      }

      /// <inheritdoc />
      public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
      {
         User? dbUser = await _dbContext.Users.Where(x => x.Id == user.Id)
                                     .Include(x => x.ContactDetail)
                                     .FirstOrDefaultAsync(cancellationToken);
         if (dbUser != null) {
            dbUser.LastName = user.LastName;
            dbUser.GivenNames = user.GivenNames;
            dbUser.ContactDetail.EmailAddress = user.ContactDetail.EmailAddress;
            dbUser.ContactDetail.MobileNumber = user.ContactDetail.MobileNumber;
            await _dbContext.SaveChangesAsync();
         }
         return dbUser; 
      }

      /// <inheritdoc />
      public async Task<User?> DeleteAsync(int id, CancellationToken cancellationToken = default)
      {
         User user = new User() { Id = id };
         _dbContext.Users.Attach(user);
         _dbContext.Users.Remove(user);
         await _dbContext.SaveChangesAsync();
         return user;
      }

      /// <inheritdoc />
      public async Task<int> CountAsync(CancellationToken cancellationToken = default)
      {
         return _dbContext.Users.Count();
      }
   }
}
