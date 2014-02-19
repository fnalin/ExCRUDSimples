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

        //Qtde exibida por vez na paginação
        // O ideal é mantermos num xml ou no próprio banco numa sessão de "Preferencias"
        const int qtdeItensNoBloco = 5;

        public PartialViewResult GetEmpresa(int bloco)
        {
            //minha base tá no Azure, e mesmo sendo nos EUA tá muito rápido :)
            //se quiser  simular o comportamento do retorno do json em ambiente não tão rápido:
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
                WebSite = find.WebSite,
                Contatos = find.Contatos.Select(d => new ContatoVM
                {
                    ID = d.ID,
                    Nome = d.Nome,
                    Telefone = d.Telefone,
                    Celular = d.Celular,
                    Email = d.Email
                }).ToList()
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
                        WebSite = empresa.WebSite,
                    });

                    /*faltou implementar o add e edit do post do obj contato
                    
                     *  ficaria mais ou menos assim:
                     
                     */

                    //empresa.Contatos.ToList().ForEach(d =>
                    //    _db.Contatos.Add(new Contato
                    //    {
                    //        Nome = d.Nome,
                    //        Telefone = d.Telefone,
                    //        Celular = d.Celular,
                    //        Email = d.Email
                    //    })
                    //);
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

                //empresa.CodigoFonteTagRels.ToList()
                //    .ForEach(d => db.CodigoFonteTagRels.Remove(db.CodigoFonteTagRels.Find(d.ID)));

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

            //minha base tá no Azure, e mesmo sendo nos EUA tá muito rápido :)
            //se quiser  simular o comportamento do retorno do json em ambiente não tão rápido:
            //System.Threading.Thread.Sleep(3000);

            return Json(mensagemErro, JsonRequestBehavior.DenyGet);

        }


        #region Métodos ref. Contato(s)

        public JsonResult GetContato(int id)
        {

            string mensagemErro = string.Empty;
            Contato contato = null;

            try
            {
                contato = _db.Contatos.Find(id);

                if (contato == null)
                {
                    throw new Exception("Contato não localizado!");
                    //mensagemErro = "Contato não localizado!";
                }

            }
            catch (Exception ex)
            {

                mensagemErro = ex.Message;

            }

            return Json(
                new
                {
                    erro = mensagemErro,
                    dados = contato != null ? new
                    {
                        id = contato.ID,
                        nome = contato.Nome,
                        telefone = contato.Telefone,
                        celular = contato.Celular,
                        email = contato.Email
                    } : null,
                },
                JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetContatos(int id)
        {
            //minha base tá no Azure, e mesmo sendo nos EUA tá muito rápido :)
            //se quiser  simular o comportamento do retorno do json em ambiente não tão rápido:
            //System.Threading.Thread.Sleep(2000);
            var contatos = _db.Empresas.Find(id).Contatos.Select(d => new ContatoVM()
            {
                ID = d.ID,
                Nome = d.Nome,
                Telefone = d.Telefone,
                Celular = d.Celular,
                Email = d.Email
            });
            var view = "_DadosContatoView";
            return PartialView(view, contatos);
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
        #endregion


        #region Dispose
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
        #endregion

    }
}