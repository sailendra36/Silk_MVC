using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silk.DataAccess.DbInitializer
{
	public interface IDbInitializer
	{
		//RESPONSIBLE FOR CREATING ADMIN USER AND ROLES IN PRODUCTION DEPLOYMENT
		void Initialize();
	}
}
