//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FutureExpress.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public int OrderID { get; set; }
        public byte OrderStatusID { get; set; }
        public System.DateTime OrderCreateDate { get; set; }
        public System.DateTime OrderDeliveryDate { get; set; }
        public int OrderPickupPointID { get; set; }
        public string UserName { get; set; }
        public int GetCode { get; set; }
        public Nullable<int> RateId { get; set; }
        public Nullable<int> Weight { get; set; }
    
        public virtual OrderStatu OrderStatu { get; set; }
        public virtual PickupPoint PickupPoint { get; set; }
        public virtual Rate Rate { get; set; }
        public virtual User User { get; set; }
    }
}