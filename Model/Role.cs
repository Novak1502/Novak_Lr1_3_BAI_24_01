using System.Collections.Generic;
using System.ComponentModel;

namespace Novak_Lr1_3_BAI_24_01.Model
{
    public class Role
    {
        public int Id { get; set; }
        public string NameRole { get; set; }

        public Role()
        {
            this.Persons = new HashSet<Person>();
        }

        // Коллекция для связи с сотрудниками (1 ко многим)
        public virtual ICollection<Person> Persons { get; set; }

        // Метод ShallowCopy больше не нужен, так как мы работаем с БД напрямую,
        // но если XAML где-то его использует, можно оставить пустым или удалить.
        public Role ShallowCopy() { return (Role)this.MemberwiseClone(); }
    }
}