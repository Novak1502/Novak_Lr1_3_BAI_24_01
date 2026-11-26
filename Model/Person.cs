using System;

namespace Novak_Lr1_3_BAI_24_01.Model
{
    public class Person
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }

        // Связь с должностью
        public virtual Role Role { get; set; }
    }
}