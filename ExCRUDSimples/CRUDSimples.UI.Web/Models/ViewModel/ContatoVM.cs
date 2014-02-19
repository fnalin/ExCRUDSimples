using System.ComponentModel.DataAnnotations;

/*
 * Criei essa VM somente para não sujar as classes de domínio ...  um pouco de boas práticas é bom né? :)
 */

namespace CRUDSimples.UI.Web.Models.ViewModel
{
    public class ContatoVM
    {
        [Required()]
        public int ID { get; set; }
        [Required()]
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public EmpresaVM Empresa { get; set; }

    }
}