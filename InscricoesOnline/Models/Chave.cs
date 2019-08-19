using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    [Table("TBL_CHAVE")]
    public class Chave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long EventoId { get; set; }
        public virtual Evento Evento { get; set; }

        [MaxLength(10)]
        public string Sexo { get; set; }

        public long ModalidadeId { get; set; }
        public virtual Modalidade Modalidade { get; set; }

        public long? CategoriaFaixaId { get; set; }
        public virtual CategoriaFaixa CategoriaFaixa { get; set; }

        public long? CategoriaIdadeId { get; set; }
        public virtual CategoriaIdade CategoriaIdade { get; set; }

        public long? CategoriaLutaPesoId { get; set; }
        public virtual CategoriaLutaPeso CategoriaLutaPeso { get; set; }

        public virtual List<ChaveParticipante> ChaveParticipantes { get; set; }
        public virtual List<InscricaoModalidade> Inscritos { get; set; }
    }
}