using Novak_Lr1_3_BAI_24_01.Model;

namespace Novak_Lr1_3_BAI_24_01.Helper
{
    public class FindRole
    {
        private int id;

        public FindRole(int id)
        {
            this.id = id;
        }

        public bool RolePredicate(Role role)
        {
            return role.Id == id;
        }
    }
}