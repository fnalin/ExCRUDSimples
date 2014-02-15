using CRUDSimples.UI.Web.Models;
using CRUDSimples.UI.Web.Models.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CRUDSimples.UI.Web.Controllers
{
    public class CadastroController : Controller
    {
        private Contexto _db;
        public CadastroController()
        {
            _db = new Contexto();
        }

        public ActionResult Index()
        {
            ViewBag.update = (bool)(TempData["update"] ?? false);
            ViewBag.bloco = (string)(TempData["bloco"] ?? "");
            return View();
        }

        const int qtdeItensNoBloco = 5;//Qtde exibida por vez na paginação

        public PartialViewResult GetEmpresa(int bloco)
        {
            //System.Threading.Thread.Sleep(2000);
            if (bloco == 0) //se p bloco for 0 então ir para o último (esse valor vem da interface, qdo o cara cria um novo)
            {
                var total = _db.Empresas.Count();
                bloco = (total / qtdeItensNoBloco);
                if (total % qtdeItensNoBloco > 0)
                    bloco++;
            }

            var skip = ((bloco * qtdeItensNoBloco) - (qtdeItensNoBloco - 1)) - 1;
            var dados = _db.Empresas.OrderBy(d => d.ID).Skip(skip).Take(qtdeItensNoBloco);
            var view = "_RelacaoEmpresas";
            ViewBag.Titulo = "Empresas";
            return PartialView(view, dados);
        }

        public PartialViewResult GetPaginacao()
        {
            var total = _db.Empresas.Count();

            var _blocoTotal = (total / qtdeItensNoBloco);
            if (total % qtdeItensNoBloco > 0)
                _blocoTotal++;

            ViewBag.total = _blocoTotal;
            return PartialView("_MenuPaginacao");
        }

        public PartialViewResult GetContatos(int id)
        {
            //System.Threading.Thread.Sleep(2000);
            //throw new NotImplementedException("não!");

            var contatos = _db.Empresas.Find(id).Contatos;
            var view = "_DadosContatoView";
            return PartialView(view, contatos);
        }

        public ActionResult Add()
        {
            return View("AddEdit", new EmpresaVM());
        }

        public ActionResult Edit(int id)
        {

            var find = _db.Empresas.Find(id);

            if (find == null)
                throw new Exception("Empresa não existe!");

            var empresa = new EmpresaVM
            {
                ID = find.ID,
                RazaoSocial = find.Nome,
                NomeFantasia = find.NomeFantasia,
                CNPJ = find.CNPJ,
                WebSite = find.WebSite
            };

            return View("AddEdit", empresa);
        }

        [HttpPost]
        public ActionResult AddEdit(EmpresaVM empresa)
        {
            if (ModelState.IsValid)
            {
                // No id so we add it to database
                if (empresa.ID <= 0)
                {
                    _db.Empresas.Add(new Empresa
                    {
                        Nome = empresa.RazaoSocial,
                        NomeFantasia = empresa.NomeFantasia,
                        CNPJ = empresa.CNPJ,
                        WebSite = empresa.WebSite
                    });
                }
                else
                {
                    var edit = _db.Empresas.Find(empresa.ID);
                    edit.Nome = empresa.RazaoSocial;
                    edit.NomeFantasia = empresa.NomeFantasia;
                    edit.CNPJ = empresa.CNPJ;
                    edit.WebSite = empresa.WebSite;

                    //descobrir o bloco do cara e mandá-lo para a interface
                    var posicao = _db.Empresas.Where(d => d.ID <= empresa.ID).OrderBy(d => d.ID).Count();
                    TempData["bloco"] = (posicao / qtdeItensNoBloco).ToString();
                }
                _db.SaveChanges();
                TempData["update"] = true;
                return RedirectToAction("Index");

            }

            return View(empresa);


        }

        [HttpPost]
        public JsonResult DeleteEmpresa(int id)
        {
            var mensagemErro = string.Empty;

            try
            {
                var empresa = _db.Empresas.Find(id);
                if (empresa == null)
                    throw new Exception("Fonte não localizado!");

                _db.Empresas.Remove(empresa);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    //capturando o erro da FK - existem ainda contatos na empresa!
                    if (ex.InnerException.ToString().Contains("FK_tbDemoCrudContato_EmpresaID"))
                    {
                        mensagemErro = "Para excluir é necessário que não haja nenhum contato nesta empresa!";
                    }
                    else
                    {
                        mensagemErro = ex.InnerException.Message;
                    }
                }
                else
                {
                    mensagemErro = ex.Message;
                }
            }

            //System.Threading.Thread.Sleep(3000);

            return Json(mensagemErro, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public JsonResult SaveContato(Contato dados)
        {
            string mensagemErro = null;

            try
            {
                _db.Contatos.Add(dados);
                _db.SaveChanges();
            }
            // ** TO DO Tratar para receber no json **
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        //mensagemErro += string.Format("A Entidade {0}, no estado {1} gerou os seguinte(s) erro(s) de validação: \n",
            //        //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        //foreach (var ve in eve.ValidationErrors)
            //        //{
            //        //     mensagemErro += string.Format("- Propriedade: {0}, Erro: {1}\n",
            //        //        ve.PropertyName, ve.ErrorMessage);
            //        //}
            //    }
            //}
            catch (Exception ex)
            {

                mensagemErro = ex.Message;

            }

            return Json(
                mensagemErro
                , JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public JsonResult DeleteContato(int id)
        {
            string mensagemErro = null;
            try
            {
                var contato = _db.Contatos.Find(id);
                _db.Contatos.Remove(contato);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    mensagemErro = ex.InnerException.Message;
                }
                else
                {
                    mensagemErro = ex.Message;
                }
            }
            return Json(
                mensagemErro
                , JsonRequestBehavior.DenyGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}