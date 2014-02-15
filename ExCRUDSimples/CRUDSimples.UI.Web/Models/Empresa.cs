using System.Collections.Generic;

namespace CRUDSimples.UI.Web.Models
{
    public class Empresa
    {
        public Empresa()
        {
            this.Contatos = new List<Contato>();
        }

        public int ID { get; set; }
        public string CNPJ { get; set; }
        public string Nome { get; set; }
        public string NomeFantasia { get; set; }
        public string WebSite { get; set; }
        public virtual ICollection<Contato> Contatos { get; set; }
    }
}