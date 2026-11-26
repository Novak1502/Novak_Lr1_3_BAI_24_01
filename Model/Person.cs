using System;

namespace Novak_Lr1_3_BAI_24_01.Model
{
    public class Person
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }

        public Person() { }

        public Person(int id, int roleId, string firstName, string lastName, string birthday)
        {
            this.Id = id;
            this.RoleId = roleId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Birthday = birthday;
        }

        public Person CopyFromPersonDpo(PersonDpo dpo)
        {
            Person p = new Person();
            p.Id = dpo.Id;
            p.FirstName = dpo.FirstName;
            p.LastName = dpo.LastName;
            p.Birthday = dpo.Birthday;
            return p;
        }
    }
}