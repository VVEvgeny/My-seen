//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace My_Seen
{
    using System;
    using System.Collections.Generic;
    
    public partial class Films_New
    {
        public long Id { get; set; }
        public int UsersId { get; set; }
        public string Name { get; set; }
        public System.DateTime DateSee { get; set; }
        public int Rate { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
