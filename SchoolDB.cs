using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Entity_Framework_Data_Binding
{
    public partial class SchoolDB : DbContext
    {
        // T?o DbSet ?? qu?n lý các ??i t??ng Student
        public virtual DbSet<Student> Students { get; set; }

        // Hàm d?ng m?c ??nh c?a DbContext, s? d?ng connection string "SchoolDB"
        public SchoolDB()
            : base("name=SchoolDB") // Truy?n connection string t? file c?u hình (App.config)
        {
        }
    }
}
