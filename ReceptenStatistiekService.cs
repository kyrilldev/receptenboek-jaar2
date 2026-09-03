using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receptenboek
{
    public class ReceptenStatistiekService
    {
        private readonly IReceptenRepository _repository;

        public ReceptenStatistiekService(IReceptenRepository repository)
        {
            _repository = repository;
        }

        public float BerekenGemiddeldeBereidingstijd()
        {
            var recepten = _repository.GetAlleRecepten();
            float total = 0;
            for (int i = 0; i < recepten.Count; i++)
            {
                //pak de gemiddelde bereidingstijd per recept
                total += recepten[i].BerekenTotaleBereidingsTijd();
            }

            //bereken daarmee het gemiddelde
            if (recepten.Count > 0)
            {
                return total / recepten.Count;
            }
            else return 0;
            
        }
    }
}
