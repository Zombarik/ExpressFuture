using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FutureExpress.Models
{
    public partial class DataDBEntities: DbContext
    {
        private static DataDBEntities _context;
        
        public static DataDBEntities GetContext()
        {
            if (_context == null)
            { _context = new DataDBEntities();
                //_context.Configuration.LazyLoadingEnabled = true;
                //_context.Configuration.ProxyCreationEnabled = true;
                //_context.Configuration.AutoDetectChangesEnabled = true;
            }
            return _context;
        }
    }
}
