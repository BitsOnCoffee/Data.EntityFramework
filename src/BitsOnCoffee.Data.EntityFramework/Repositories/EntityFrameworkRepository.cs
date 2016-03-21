using BitsOnCoffee.Data.EntityFramework.Context;
using BitsOnCoffee.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;

namespace BitsOnCoffee.Data.EntityFramework.Repositories
{
	public class EntityFrameworkRepository : RepositoryBase, IRepository
	{
		public override void Add<TEntity>(TEntity item)
		{
			GetEntries<TEntity>().Add(item);
		}

		public override void Delete<TEntity>(TEntity item)
		{
			GetEntityFrameworkContextBase().Entry<TEntity>(item).State = System.Data.Entity.EntityState.Deleted;
		}

		public override void Delete<TEntity>(long id)
		{
			TEntity item = Get<TEntity>(id);
			GetEntityFrameworkContextBase().Entry<TEntity>(item).State = System.Data.Entity.EntityState.Deleted;
		}

		public override void Edit<TEntity>(TEntity item)
		{
			var context = GetEntityFrameworkContextBase();

			DbEntityEntry dbEntityEntry = context.Entry(item);
			if (dbEntityEntry.State == EntityState.Detached)
			{
				context.Set<TEntity>().Attach(item);
			}

			context.Entry<TEntity>(item).State = System.Data.Entity.EntityState.Modified;
		}

		public override TEntity Get<TEntity>()
		{
			return GetEntries<TEntity>().FirstOrDefault();
		}

		public override TEntity Get<TEntity>(long id)
		{
			return GetEntries<TEntity>().Find(id);
		}

		public override TEntity Get<TEntity>(long id, params Expression<Func<TEntity, object>>[] relatedEntities)
		{
			DbSet<TEntity> dbSet = GetEntries<TEntity>();
			foreach (var item in relatedEntities)
			{
				dbSet.Include(item);
			}
			return dbSet.Find(id);
		}

		public override TResult Get<TEntity, TResult>(long id, Expression<Func<TEntity, TResult>> columns)
		{
			DbSet<TEntity> dbSet = GetEntries<TEntity>();
			return dbSet.Where(s => s.Id == id).Select(columns).FirstOrDefault();
		}

		public override IQueryable<TEntity> GetAll<TEntity>()
		{
			return GetEntries<TEntity>();
		}

		public override IQueryable<TEntity> GetAll<TEntity>(params Expression<Func<TEntity, object>>[] relatedEntities)
		{
			DbSet<TEntity> dbSet = GetEntries<TEntity>();
			foreach (var item in relatedEntities)
			{
				dbSet.Include(item);
			}
			return dbSet;
		}

		public override void Reload<TEntity>(TEntity item)
		{
			GetEntityFrameworkContextBase().Entry<TEntity>(item).Reload();
		}

		public override long TotalCount<TEntity>()
		{
			return GetEntries<TEntity>().Count();
		}

		protected EntityFrameworkContextBase GetEntityFrameworkContextBase()
		{
			return this.Context as EntityFrameworkContextBase;
		}

		protected DbSet<TEntity> GetEntries<TEntity>() where TEntity : EntityBase
		{
			var context = GetEntityFrameworkContextBase();
			return context.Set<TEntity>();
		}
	}
}
