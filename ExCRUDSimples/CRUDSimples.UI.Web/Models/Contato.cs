namespace CRUDSimples.UI.Web.Models
{
    public class Contato
    {
        public int ID { get; set; }
        public int EmpresaID { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}