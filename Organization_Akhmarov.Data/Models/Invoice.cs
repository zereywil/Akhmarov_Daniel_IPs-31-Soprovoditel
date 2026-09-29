using System;
using System.Collections.ObjectModel;

namespace Organization_Akhmarov.Data.Models
{
    public enum InvoiceType{
        Incoming,
        Outgoing
    }

    public class Invoice
    {
        public int Id { get; set; }
        public long ScladId { get; set; }
        public DateTime Date { get; set; }
        public InvoiceType Type { get; set; }
        public bool IsConfirmed { get; set; }
        public ObservableCollection<InvoiceItem> Items { get; set; }

        public Invoice(){
            Date = DateTime.Now;
            IsConfirmed = false;
            Items = new ObservableCollection<InvoiceItem>();
        }

        public Invoice(InvoiceType type, long scladId) : this()
        {
            Type = type;
            ScladId = scladId;
        }

        public override string ToString()
        {
            string typeName = Type == InvoiceType.Incoming ? "Приход" : "Расход";
            return $"{typeName} накладная №{Id} от {Date:dd.MM.yyyy}";
        }
    }
}