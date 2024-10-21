using Entity_Framework_Data_Binding;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Framework_Data_Binding
{
    public class SchoolContext : DbContext
    {
        // Tạo DbSet để quản lý các đối tượng Student
        public DbSet<Student> Students { get; set; }

        // Hàm dựng (constructor) với tên connection string từ App.config
        public SchoolContext()
            : base("name=SchoolDB") // Dùng connection string "SchoolDB"
        {
        }
    }
}
