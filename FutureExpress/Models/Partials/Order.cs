using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FutureExpress.Models
{
    public partial class Order
    {
        public string GetColor
        {
            get
            {
                return $"{OrderStatu.Color}";
            }
        }

        public string GetUser
        {
            get
            {
                 return $"{User.Clients.Single().Surname} {User.Clients.Single().Name} телефон: {User.Clients.Single().Phone}";
            }
        }

        public string GetPhoto
        {
            get
            {
                return User.Clients.Single().GetPhoto;
            }
        }

        public Visibility GetVisibility
        {
            get
            {
                if
                (Manager.CurrentUser.RoleId == 3)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }
    }
}
