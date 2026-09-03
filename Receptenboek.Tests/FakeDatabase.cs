using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receptenboek.Tests
{
    internal class FakeDatabase : IReceptenRepository
    {
        private List<Recept> _recepten = new();

        public List<Recept> GetAlleRecepten()
        {
            return _recepten;
        }

        public void VoegReceptToe(Recept recept)
        {
            _recepten.Add(recept);
        }
    }
}
