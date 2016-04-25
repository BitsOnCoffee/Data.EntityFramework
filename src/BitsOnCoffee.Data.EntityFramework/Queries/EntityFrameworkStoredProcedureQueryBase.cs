using BitsOnCoffee.Data.Queries;
using BitsOnCoffee.Data.StoredProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsOnCoffee.Data.EntityFramework.Queries
{
	public abstract class EntityFrameworkStoredProcedureQueryBase<TStoredProcedure> : QueryBase<TStoredProcedure> where TStoredProcedure : StoredProcedureBase
	{
		protected IList<TStoredProcedure> CallStoredProcedure(TStoredProcedure storedProcedure)
		{
			return this.Context.CallStoredProcedure<TStoredProcedure>(storedProcedure).ToList();
		}
	}
}
