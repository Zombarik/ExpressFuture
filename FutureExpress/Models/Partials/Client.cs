using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutureExpress.Models
{
    public partial class Client
    {
        public string GetPhoto
        {
            get
            {
                if (Photo is null)
                    return System.IO.Directory.GetCurrentDirectory() + @"\Images\picture.png";
                return System.IO.Directory.GetCurrentDirectory() + @"\Images\" + Photo.Trim();

            }
        }
        public string GetFio
        {
            get
            {
                return $"{Surname} {Name} ";
            }
        }
    }
}
