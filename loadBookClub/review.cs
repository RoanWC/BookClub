//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace loadBookClub
{
    using System;
    using System.Collections.Generic;
    
    public partial class review
    {
        public int REVIEW_ID { get; set; }
        public int BOOK_ID { get; set; }
        public string USERNAME { get; set; }
        public Nullable<int> RATING { get; set; }
        public string CONTENT { get; set; }
    
        public virtual user user { get; set; }
        public virtual book book { get; set; }
    }
}
