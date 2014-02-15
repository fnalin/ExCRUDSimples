using System.ComponentModel.DataAnnotations;

namespace CRUDSimples.UI.Web.Models.ViewModel
{
    public class EmpresaVM
    {
        [Required()]
        public int ID { get; set; }
        [Required()]
        public string RazaoSocial { get; set; }
        [Required()]
        public string NomeFantasia { get; set; }
        [Required()]
        public string CNPJ { get; set; }
        //[RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+[.com]+(/[/?%&=]*)?")]
        public string WebSite { get; set; }
    }
}