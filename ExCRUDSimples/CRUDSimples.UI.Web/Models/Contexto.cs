using System.Data.Entity;
using System.Reflection;

namespace CRUDSimples.UI.Web.Models
{
    public class Contexto : DbContext
    {
        static Contexto()
        {
            Database.SetInitializer<Contexto>(null);
        }

        //Atenção --
        //Estou usando o SQLExpress em minha máquina via Windows Autentication
        //Colocar aqui a Connection String ou referenciá-la com o nome da Conexão no Web.Config. Ex: LocalSQLExp
        public Contexto()
            : base(@"Data Source=.\sqlexpress;Initial Catalog=CadEmpresaContatos;Integrated Security=True;MultipleActiveResultSets=True")
        //: base("Name=LocalSQLExp")
        { }

        public DbSet<Contato> Contatos { get; set; }
        public DbSet<Empresa> Empresas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Cria via Reflection os mapeamentos que estão no Assembly - EF6
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
