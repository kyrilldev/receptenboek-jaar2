using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receptenboek
{
    public interface IReceptenRepository
    {
        public List<Recept> GetAlleRecepten();
        public void VoegReceptToe(Recept recept);
    }
}
