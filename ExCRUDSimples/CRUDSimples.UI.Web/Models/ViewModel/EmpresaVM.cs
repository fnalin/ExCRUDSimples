using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/*
 * Criei essa VM somente para não sujar as classes de domínio ...  um pouco de boas práticas é bom né? :)
 */

namespace CRUDSimples.UI.Web.Models.ViewModel
{
    public class EmpresaVM
    {

        public EmpresaVM()
        {
            this.Contatos = new List<ContatoVM>();
        }

        [Required()]
        public int ID { get; set; }
        [Required()]
        public string RazaoSocial { get; set; }
        [Required()]
        public string NomeFantasia { get; set; }
        [Required()]
        public string CNPJ { get; set; }
        //eu sei, tá feio né... se contratado crio vergonha e altero ok? :)
        //[RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+[.com]+(/[/?%&=]*)?")]
        public string WebSite { get; set; }

        public ICollection<ContatoVM> Contatos { get; set; }
    }
}