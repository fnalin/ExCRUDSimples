using System.Data.Entity.ModelConfiguration;

namespace CRUDSimples.UI.Web.Models.Mapping
{
    public class EmpresaMap : EntityTypeConfiguration<Empresa>
    {
        public EmpresaMap()
        {
            // PK
            this.HasKey(t => t.ID);

            // Propriedade C#
            this.Property(t => t.CNPJ)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Nome)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.NomeFantasia)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.WebSite)
                .HasMaxLength(60);

            // Mapeamento da Tabela e suas colunas
            this.ToTable("tbDemoCrudEmpresa");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CNPJ).HasColumnName("CNPJ");
            this.Property(t => t.Nome).HasColumnName("Nome");
            this.Property(t => t.NomeFantasia).HasColumnName("NomeFantasia");
            this.Property(t => t.WebSite).HasColumnName("WebSite");
        }
    }
}