using System;
using System.Collections.Generic;
using Receptenboek.Domain;

namespace Receptenboek.Application.Interfaces
{
    public interface IReceptenRepository
    {
        List<Recept> GetAlleRecepten();
        void VoegReceptToe(Recept recept);
        bool VerwijderRecept(Recept recept);
    }
}
