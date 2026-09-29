using NUnit.Framework;
using Organization_Akhmarov.Data.Models;
using Organization_Akhmarov.Data.Services;
using System.Linq;

namespace Organization_Akhmarov.Tests.ServicesTests{
    [TestFixture]
    public class ScladServiceTests
    {
        private ScladService _service;
        private OrganizationService _orgService;

        [SetUp]
        public void Setup(){
            _orgService = new OrganizationService();
            _service = new ScladService(_orgService);
        }

        [Test]
        public void GetAll_ShouldReturnSclads()
        {
            var result = _service.GetAll();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0));
        }

        [Test]
        public void GetByOrganizationId_ShouldReturnCorrectSclads()
        {
            var firstOrg = _orgService.GetAll().First();
            var sclads = _service.GetByOrganizationId(firstOrg.Id);
            Assert.That(sclads, Is.Not.Null);
            foreach (var sclad in sclads)
            {
                Assert.That(sclad.OrganizationId, Is.EqualTo(firstOrg.Id));
            }
        }

        [Test]
        public void Add_ShouldIncreaseCount()
        {
            var firstOrg = _orgService.GetAll().First();
            var initialCount = _service.GetAll().Count;
            var newSclad = new Sclad("Тест склад", "Тест адрес", firstOrg.Id);
            _service.Add(newSclad);
            var newCount = _service.GetAll().Count;
            Assert.That(newCount, Is.EqualTo(initialCount + 1));
        }

        [Test]
        public void GetById_ShouldReturnCorrectSclad()
        {
            var firstSclad = _service.GetAll().First();
            var result = _service.GetById(firstSclad.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(firstSclad.Id));
        }

        [Test]
        public void Delete_ShouldRemoveSclad()
        {
            var firstOrg = _orgService.GetAll().First();
            var newSclad = new Sclad("на удаление", "Адрес", firstOrg.Id);
            _service.Add(newSclad);
            var scladId = newSclad.Id;
            var initialCount = _service.GetAll().Count;
            _service.Delete(scladId);
            var newCount = _service.GetAll().Count;
            Assert.That(newCount, Is.EqualTo(initialCount - 1));
            Assert.That(_service.GetById(scladId), Is.Null);
        }
    }
}