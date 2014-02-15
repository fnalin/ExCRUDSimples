using System.Data.Entity.ModelConfiguration;

namespace CRUDSimples.UI.Web.Models.Mapping
{
    public class ContatoMap : EntityTypeConfiguration<Contato>
    {
        public ContatoMap()
        {
            // PK
            this.HasKey(t => t.ID);

            // Propriedades
            this.Property(t => t.Nome)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.Telefone)
                .HasMaxLength(14);

            this.Property(t => t.Celular)
                .HasMaxLength(14);

            this.Property(t => t.Email)
                .HasMaxLength(60);

            // Mapeamento da Tabela e campos
            this.ToTable("tbDemoCrudContato");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.EmpresaID).HasColumnName("EmpresaID");
            this.Property(t => t.Nome).HasColumnName("Nome");
            this.Property(t => t.Telefone).HasColumnName("Telefone");
            this.Property(t => t.Celular).HasColumnName("Celular");
            this.Property(t => t.Email).HasColumnName("Email");

            // Relacionamento muitos p/ 1 (Contato -> Empresa)
            this.HasRequired(t => t.Empresa)
                .WithMany(t => t.Contatos)
                .HasForeignKey(d => d.EmpresaID);

        }
    }
}