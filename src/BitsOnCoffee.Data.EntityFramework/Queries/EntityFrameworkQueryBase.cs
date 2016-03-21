using BitsOnCoffee.Data.EntityFramework.Context;
using BitsOnCoffee.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsOnCoffee.Data.EntityFramework.Queries
{
	public abstract class EntityFrameworkQueryBase<TEntity, TReturn> : QueryBase<TEntity, TReturn> where TEntity : EntityBase
	{
		protected DbSet<TEntity> Entries
		{
			get
			{
				var context = this.Context as EntityFrameworkContextBase;
				return context.Set<TEntity>();
			}
		}
	}

	public abstract class EntityFrameworkQueryBase<TEntity> : QueryBase<TEntity, TEntity> where TEntity : EntityBase
	{
		protected DbSet<TEntity> Entries
		{
			get
			{
				var context = this.Context as EntityFrameworkContextBase;
				return context.Set<TEntity>();
			}
		}
	}
}
