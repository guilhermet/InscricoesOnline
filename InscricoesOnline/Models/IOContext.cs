using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    [DbConfigurationType(typeof(MyDbConfiguration))]
    public class IOContext : DbContext
    {
        //public IOContext() : base("USER ID=InscricoesOnline;Password=InscricoesOnline#XYFJL127TT4MCINLUK;Data Source=XE;")
        //{

        //}

        public IOContext() : base()
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("InscricoesOnline");

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConve‌​ntion>();
        }

        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Atleta> Atletas { get; set; }
        public DbSet<Faixa> Faixas { get; set; }
        
        //public DbSet<Noticia> Noticias { get; set; }
        public DbSet<Evento> Eventos { get; set; }

        public DbSet<Modalidade> Modalidades { get; set; }
        public DbSet<CategoriaIdade> CategoriaIdades { get; set; }
        public DbSet<CategoriaLutaPeso> CategoriaLutaPeso { get; set; }
        public DbSet<CategoriaFaixa> CategoriaFaixas { get; set; }
        public DbSet<Inscricao> Inscricoes { get; set; }
        public DbSet<InscricaoModalidade> InscricoesModalidades { get; set; }
        public DbSet<Chave> Chaves { get; set; }
        public DbSet<ChaveParticipante> ChaveParticipantes { get; set; }
        
        //public DbSet<Usuario> Usuarios { get; set; }
    }
}
