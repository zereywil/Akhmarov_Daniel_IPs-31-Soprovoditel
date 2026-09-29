using System.Collections.Generic;
using System.Linq;
using Organization_Akhmarov.Data.Models;

namespace Organization_Akhmarov.Data.Services{
    public class ScladService : IRepository<Sclad>
    {
        private List<Sclad> _sclads;
        private OrganizationService _orgService;

        public ScladService(OrganizationService orgService){
            _orgService = orgService;
            var organizations = _orgService.GetAll();
            _sclads = DataGenerator.GenerateSclads(organizations);
        }

        public List<Sclad> GetAll(){
            return _sclads.ToList();
        }

        public List<Sclad> GetByOrganizationId(long organizationId){
            return _sclads.Where(w => w.OrganizationId == organizationId).ToList();
        }

        public Sclad? GetById(object id){
            if (id is long longId)
                return _sclads.FirstOrDefault(w => w.Id == longId);
            return null;
        }

        public void Add(Sclad entity)
        {
            _sclads.Add(entity);
        }

        public void Update(Sclad entity)
        {
            var index = _sclads.FindIndex(w => w.Id == entity.Id);
            if (index != -1)
                _sclads[index] = entity;
        }

        public void Delete(object id){
            var sclad = GetById(id);
            if (sclad != null)
                _sclads.Remove(sclad);
        }
    }
}