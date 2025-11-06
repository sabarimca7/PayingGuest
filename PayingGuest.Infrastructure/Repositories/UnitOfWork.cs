using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;

namespace PayingGuest.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PayingGuestDbContext _context;
        private IDbContextTransaction? _transaction;
        private IUserRepository? _users;
        private IRepository<Property>? _properties;
        private IRepository<AuditLog>? _auditLogs;
        private IRepository<ClientToken>? _clientTokens;
        public IMenuRepository _menus;
        public IUserRoleRepository _userRoles;
        public IUserTokenRepository _userTokens;
        public UnitOfWork(PayingGuestDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IMenuRepository Menus => _menus ??= new MenuRepository(_context);
        public IUserRoleRepository UserRoles => _userRoles ??= new UserRoleRepository(_context);
        public IUserTokenRepository UserTokens => _userTokens ??= new UserTokenRepository(_context);
        public IRepository<Property> Properties => _properties ??= new Repository<Property>(_context);
        public IRepository<AuditLog> AuditLogs => _auditLogs ??= new Repository<AuditLog>(_context);
        public IRepository<ClientToken> ClientTokens => _clientTokens ??= new Repository<ClientToken>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}