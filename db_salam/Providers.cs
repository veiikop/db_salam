//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace db_salam
{
    using System;
    using System.Collections.Generic;
    
    public partial class Providers
    {
        public int Provider_ID { get; set; }
        public int Product_ID { get; set; }
        public int Storage_ID { get; set; }
        public Nullable<System.DateTime> DateOfProvide { get; set; }
    
        public virtual Products Products { get; set; }
        public virtual Storage Storage { get; set; }
    }
}
