using System.Data.Entity;
using Novak_Lr1_3_BAI_24_01.Model;

namespace Novak_Lr1_3_BAI_24_01
{
    public class CompanyEntities : DbContext
    {
        public CompanyEntities() : base("name=CompanyEntities")
        {
        }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
    }
}