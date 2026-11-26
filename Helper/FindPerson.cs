using Novak_Lr1_3_BAI_24_01.Model;

namespace Novak_Lr1_3_BAI_24_01.Helper
{
    public class FindPerson
    {
        private int id;

        public FindPerson(int id)
        {
            this.id = id;
        }

        public bool PersonPredicate(Person person)
        {
            return person.Id == id;
        }
    }
}