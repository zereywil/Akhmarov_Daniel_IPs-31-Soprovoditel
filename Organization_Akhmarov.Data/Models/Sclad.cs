namespace Organization_Akhmarov.Data.Models
{
    public class Sclad
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public long OrganizationId { get; set; }

        public Sclad() { }

        public Sclad(string name, string address, long organizationId)
        {
            Name = name;
            Address = address;
            OrganizationId = organizationId;
        }

        public override string ToString() => Name;
    }
}