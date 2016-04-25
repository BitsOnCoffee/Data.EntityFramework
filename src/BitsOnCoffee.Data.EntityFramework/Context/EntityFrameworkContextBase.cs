using BitsOnCoffee.Data.Context;
using BitsOnCoffee.Data.StoredProcedures;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsOnCoffee.Data.EntityFramework.Context
{
	public abstract class EntityFrameworkContextBase : DbContext, IDbContext
	{
		protected readonly string nameOrConnectionString;

		#region ContextBase (ctor)
		public EntityFrameworkContextBase(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
			this.nameOrConnectionString = nameOrConnectionString;
			this.Configuration.UseDatabaseNullSemantics = true;

			SetInitializer();
		}
		#endregion

		#region CallStoredProcedure
		public IList<TStoredProcedure> CallStoredProcedure<TStoredProcedure>(TStoredProcedure storedProcedure) where TStoredProcedure : StoredProcedureBase
		{
			List<SqlParameter> parameters = new List<SqlParameter>();
			StringBuilder parameterNames = new StringBuilder();
			foreach (var param in storedProcedure.Parameters)
			{
				parameterNames.AppendFormat(" {0}", param.Key);
				parameters.Add(new SqlParameter(param.Key, param.Value));
			}

			return Database.SqlQuery<TStoredProcedure>(string.Format("{0}{1}", storedProcedure.GetType().Name, parameterNames.ToString()), parameters.ToArray()).ToList();
		}
		#endregion

		#region SetInitializer
		protected virtual void SetInitializer()
		{
			Database.SetInitializer<EntityFrameworkContextBase>(null);
		} 
		#endregion

		#region OnModelCreating
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			base.OnModelCreating(modelBuilder);
		}
		#endregion
	}

	public abstract class EntityFrameworkContextBase<TContext, TMigrationsConfiguration> : EntityFrameworkContextBase where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new() where TContext : DbContext
	{
		#region ContextBase (ctor)
		public EntityFrameworkContextBase(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
			
		}
		#endregion

		#region SetInitializer
		protected override void SetInitializer()
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<TContext, TMigrationsConfiguration>(nameOrConnectionString));
		}
		#endregion

		#region SaveChanges
		public override int SaveChanges()
		{
			try
			{
				return base.SaveChanges();
			}
			catch (DbEntityValidationException ex)
			{
				StringBuilder sb = new StringBuilder();

				foreach (var failure in ex.EntityValidationErrors)
				{
					sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
					foreach (var error in failure.ValidationErrors)
					{
						sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
					}
				}

				throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString(), ex);
			}
		} 
		#endregion
	}
}
