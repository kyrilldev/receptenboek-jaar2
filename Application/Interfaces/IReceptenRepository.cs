using System;
using System.Collections.Generic;
using Receptenboek.Domain;

namespace Receptenboek.Application.Interfaces
{
    public interface IReceptenRepository
    {
        public List<Recept> GetAlleRecepten();
        public void VoegReceptToe(Recept recept);
    }
}
